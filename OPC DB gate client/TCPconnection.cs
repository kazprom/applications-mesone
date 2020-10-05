using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPC_DB_gate_client
{
    class TCPconnection
    {

        #region VARIABLES

        private TcpClient client;
        private NetworkStream nwStream;

        private bool execution = true;
        private Thread thread_connect;
        private Thread thread_read;
        private Thread thread_write;
        private Thread thread_ping;
        private Encryption encryption = new Encryption(false);
        private Protocol protocol = new Protocol();
        byte[] buf = new byte[Protocol.SIZE_BUFFER];


        #endregion


        #region PROPERTIES

        public const string default_ip_address = "127.0.0.1";
        private Lib.Parameter<IPAddress> ip_address;
        public Lib.Parameter<IPAddress> IPaddress { get { return ip_address; } }

        public const int default_port = 1000;
        private Lib.Parameter<int> port;
        public Lib.Parameter<int> Port { get { return port; } }


        private Dictionary<int, OPC_DB_gate_Lib.TagSettings> tags = new Dictionary<int, OPC_DB_gate_Lib.TagSettings>();
        public Dictionary<int, OPC_DB_gate_Lib.TagSettings> Tags { get { return tags; } }

        #endregion


        public TCPconnection(Lib.Parameter<IPAddress> ip_address, Lib.Parameter<int> port)
        {

            this.ip_address = ip_address;
            this.ip_address.ValueChanged += Ip_address_ValueChanged;

            this.port = port;
            this.port.ValueChanged += Port_ValueChanged;

            Settings(this.ip_address.Value, this.port.Value);

        }






        #region PRIVATES


        private void Port_ValueChanged(int value)
        {
            Settings(this.ip_address.Value, value);
        }

        private void Ip_address_ValueChanged(IPAddress value)
        {
            Settings(value, this.port.Value);
        }


        private void Settings(IPAddress ip_address, int port)
        {

            if (!ip_address.Equals(this.ip_address) || port != this.port.Value)
            {

                execution = false;

                if (thread_connect != null)
                {
                    thread_connect.Join();
                    thread_connect = null;

                    Logger.WriteMessage($"Connection settings was changed from {this.ip_address.Value}:{this.port.Value} to {ip_address}:{port}. Reconnecting ...");
                }

                GC.Collect();

                this.port.Value = port;
                this.ip_address.Value = ip_address;

                execution = true;

                thread_connect = new Thread(new ThreadStart(HadlerConnect)) { IsBackground = true, Name = $"Sender" };
                thread_connect.Start();

            }
        }

        private void HadlerConnect()
        {
            try
            {
                while (execution)
                {
                    try
                    {
                        if (client == null)
                        {
                            client = new TcpClient();
                        }
                        else if (!client.Connected)
                        {
                            client = new TcpClient();
                            Lib.Encryption.SSafetyKeys sk = new Encryption.SSafetyKeys();
                            encryption.SafetyKeys = sk;
                            Logger.WriteMessage($"Try connect to server {ip_address.Value}:{port.Value} ... ");
                            client.Connect(ip_address.Value, port.Value);
                        }
                        else
                        {
                            if (nwStream == null)
                            {
                                Logger.WriteMessage($"Connected to server");
                                nwStream = client.GetStream();

                                thread_read = new Thread(new ThreadStart(HandlerRead)) { IsBackground = true, Name = $"Reader" };
                                thread_read.Start();

                                thread_write = new Thread(new ThreadStart(HandlerWrite)) { IsBackground = true, Name = $"Writer" };
                                thread_write.Start();

                                thread_ping = new Thread(new ThreadStart(HandlerPing)) { IsBackground = true, Name = $"Ping" };
                                thread_ping.Start();
                            }
                        }
                    }
                    catch (SocketException ex)
                    {
                        Logger.WriteMessage($"Can't connect to server", ex);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteMessage($"Error connect to server", ex);
                    }

                    Thread.Sleep(5000);
                }

                if (thread_read != null)
                {
                    thread_read.Join();
                    thread_read = null;
                }

                if (thread_write != null)
                {
                    thread_write.Join();
                    thread_write = null;
                }

                if (thread_ping != null)
                {
                    thread_ping.Join();
                    thread_ping = null;
                }


                if (nwStream != null)
                {
                    nwStream.Close();
                    nwStream = null;
                }

                if (client != null)
                {
                    client.Close();
                    client.Dispose();
                    client = null;
                }

                GC.Collect();

                Logger.WriteMessage($"Disconnected from server");
            }
            catch (Exception ex)
            {
                Logger.WriteMessage($"Error closing connection", ex);
            }
        }

        private void HandlerRead()
        {


            try
            {
                while (execution)
                {
                    int size_package = 0;

                    if (nwStream != null && nwStream.DataAvailable)
                        size_package = nwStream.Read(buf, 0, buf.Length);

                    if (size_package > 0)
                    {
                        byte[] income_package = new byte[size_package];
                        System.Buffer.BlockCopy(buf, 0, income_package, 0, size_package);

                        List<byte[]> packages = protocol.CatchPackage(income_package);

                        if (packages != null)
                        {
                            foreach (byte[] package in packages)
                            {
                                byte[] data = package;


                                switch (Lib.Protocol.GetTypePackage(ref data))
                                {
                                    case Protocol.EPackageTypes.UNENCRYPT:
                                        try
                                        {

                                            Encryption.SSafetyKeys safety_keys = (Encryption.SSafetyKeys)Protocol.ConvertByteArrToObj(data);
                                            if ((encryption.SafetyKeys.key == null ||
                                                !encryption.SafetyKeys.key.SequenceEqual(safety_keys.key)) &
                                                (encryption.SafetyKeys.iv == null ||
                                                !encryption.SafetyKeys.iv.SequenceEqual(safety_keys.iv)))
                                            {

                                                encryption.SafetyKeys = safety_keys;
                                                Logger.WriteMessage("Get new security key");
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.WriteMessage("Error getting package with safety keys object", ex);
                                        }
                                        break;

                                    case Protocol.EPackageTypes.ENCRYPT:
                                        try
                                        {
                                            if (client.Connected && encryption.SafetyKeys.key != null && encryption.SafetyKeys.iv != null)
                                            {
                                                object obj = Protocol.ConvertByteArrToObj(encryption.Decrypt(data));

                                                //tags
                                                if (obj.GetType().Equals(tags.GetType()))
                                                {
                                                    lock (tags)
                                                    {
                                                        Dictionary<int, OPC_DB_gate_Lib.TagSettings> fresh = (Dictionary<int, OPC_DB_gate_Lib.TagSettings>)obj;
                                                        Dictionary<int, OPC_DB_gate_Lib.TagSettings> unnecessary = new Dictionary<int, OPC_DB_gate_Lib.TagSettings>(tags);

                                                        foreach (var item in fresh)
                                                        {
                                                            if(!tags.ContainsKey(item.Key))
                                                            {
                                                                tags.Add(item.Key, item.Value);
                                                                Console.WriteLine($"Add {item.Key}");
                                                            }
                                                            else
                                                            {
                                                                tags[item.Key] = item.Value;
                                                                unnecessary.Remove(item.Key);
                                                                Console.WriteLine($"Change {item.Key}");
                                                            }
                                                        }

                                                        foreach (var item in unnecessary)
                                                        {
                                                            tags.Remove(item.Key);
                                                            Console.WriteLine($"Delete {item.Key}");
                                                        }

                                                    }
                                                }


                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.WriteMessage("Error getting package with safety keys object", ex);
                                        }
                                        break;

                                    default:
                                        Logger.WriteMessage("Unknown incoming package");
                                        break;
                                }
                            }
                        }
                    }
                    Thread.Sleep(Protocol.DATA_TIMEOUT / 2);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteMessage($"Connection error read data", ex);
                nwStream = null;
            }
        }

        private void HandlerWrite()
        {
            try
            {
                while (execution)
                {
                    if (nwStream != null)
                    {


                        /*
                        // data
                        while (Lib.Buffer.Count > 0)
                        {
                            byte[] pack = Protocol.BuildPackage(Encryption.Encrypt(Protocol.ConvertObjToByteArr(Lib.Buffer.Dequeue())), Protocol.ePackageTypes.DATA);
                            if (pack != null)
                                nwStream.Write(pack, 0, pack.Length);
                        }
                        */

                    }
                    Thread.Sleep(Protocol.DATA_TIMEOUT / 2);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteMessage($"Connection error write data", ex);
                nwStream = null;
            }

        }

        private void HandlerPing()
        {
            while (execution)
            {
                //ping
                try
                {
                    if (nwStream != null)
                    {
                        byte[] msg = { 7, 7, 7 };
                        nwStream.Write(msg, 0, msg.Length);
                    }
                }
                catch (Exception)
                {
                    Logger.WriteMessage($"Server disconnected");
                    nwStream = null;
                    return;
                }
                Thread.Sleep(1000);
            }
        }

        #endregion


    }
}
