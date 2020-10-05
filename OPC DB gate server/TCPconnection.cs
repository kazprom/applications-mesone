using Lib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace OPC_DB_gate_server
{
    public class TCPconnection : IDisposable
    {


        #region VARIABLES

        private string title;
        private bool disposedValue;
        private bool execution = true;
        private Thread thread_connect;
        private Thread thread_read;
        private Thread thread_write;
        private Thread thread_ping;
        private TcpListener listener;
        private TcpClient client;
        private NetworkStream nwStream = null;
        private Encryption encryption = new Encryption(true);
        private Protocol protocol = new Protocol();
        byte[] buf = new byte[Protocol.SIZE_BUFFER];
        private Dictionary<int, Dictionary<int, OPC_DB_gate_Lib.TagSettings>> tag_groups;

        #endregion

        #region PROPERTIES

        private int id;
        public int ID { get { return id; } }

        private IPAddress ip;
        public IPAddress IP { get { return ip; } }

        private int port;
        public int Port { get { return port; } }

        private Buffer<Protocol.SCell> buffer = new Buffer<Protocol.SCell>(1024);
        public Buffer<Protocol.SCell> Buffer { get { return buffer; } }

        #endregion

        #region CONSTRUCTOR

        public TCPconnection(int id, IPAddress ip, int port, Dictionary<int, Dictionary<int, OPC_DB_gate_Lib.TagSettings>> tag_groups)
        {
            this.id = id;
            this.tag_groups = tag_groups;
            Settings(ip, port);
        }

        #endregion

        #region DESTRUCTOR

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    listener.Stop();
                    execution = false;

                    thread_connect.Join();
                    thread_connect = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region PUBLICS

        public void Settings(IPAddress ip, int port)
        {

            if (!ip.Equals(this.ip) || port != this.port)
            {

                execution = false;

                if (thread_connect != null)
                {
                    thread_connect.Join();
                    thread_connect = null;

                    Logger.WriteMessage($"{title} changed settings to {ip}:{port}. Reconnecting ...");
                }

                GC.Collect();

                this.port = port;
                this.ip = ip;

                title = $"Listener ID<{id}> {ip}:{port}";


                execution = true;

                thread_connect = new Thread(new ThreadStart(HadlerConnect)) { IsBackground = true, Name = $"Connect ID - {id} Ip - {this.ip}:{this.port}" };
                thread_connect.Start();

            }
        }

        public void SendEncrypt(object obj)
        {
            try
            {
                if (nwStream != null)
                {

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error send table", ex);
            }
        }

        #endregion

        #region PRIVATES

        private void HadlerConnect()
        {
            try
            {
                while (execution)
                {
                    try
                    {
                        listener = new TcpListener(ip, port);
                        listener.Start();
                        Logger.WriteMessage($"{title} opened");

                        using (client = new TcpClient())
                        {
                            while (execution)
                            {
                                try
                                {

                                    if (!client.Connected)
                                    {
                                        Logger.WriteMessage($"{title} waiting ...");
                                        if (listener.Pending())
                                            client = listener.AcceptTcpClient();
                                    }
                                    else
                                    {
                                        if (nwStream == null)
                                        {
                                            nwStream = client.GetStream();
                                            Logger.WriteMessage($"{title} connected with client {((IPEndPoint)client.Client.RemoteEndPoint).Address}:{((IPEndPoint)client.Client.RemoteEndPoint).Port}");

                                            thread_read = new Thread(new ThreadStart(HandlerRead)) { IsBackground = true, Name = $"Read ID - {id} Ip - {this.ip}:{this.port}" };
                                            thread_read.Start();

                                            thread_write = new Thread(new ThreadStart(HandlerWrite)) { IsBackground = true, Name = $"Write ID - {id} Ip - {this.ip}:{this.port}" };
                                            thread_write.Start();

                                            thread_ping = new Thread(new ThreadStart(HandlerPing)) { IsBackground = true, Name = $"Ping ID - {id} Ip - {this.ip}:{this.port}" };
                                            thread_ping.Start();
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.WriteMessage($"{title} error connection", ex);
                                }

                                Thread.Sleep(5000);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteMessage($"{title} error opening", ex);
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

                if (listener != null)
                {
                    listener.Server.Close();
                    listener.Stop();
                    listener = null;
                }

                GC.Collect();

                Logger.WriteMessage($"{title} closed");
            }
            catch (Exception ex)
            {
                Logger.WriteMessage($"{title} error closing", ex);
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
                    {
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

                                    switch (Protocol.GetTypePackage(ref data))
                                    {
                                        case Protocol.EPackageTypes.ENCRYPT:
                                            try
                                            {
                                                object obj = Protocol.ConvertByteArrToObj(encryption.Decrypt(data));

                                                if (obj.GetType().Equals(typeof(Protocol.SCell)))
                                                {
                                                    buffer.Enqueue((Protocol.SCell)obj);
                                                }


                                            }
                                            catch (Exception ex)
                                            {
                                                Logger.WriteMessage($"{title} error getting package with data object", ex);
                                            }
                                            break;
                                    }
                                }
                            }

                        }
                    }
                }

                Thread.Sleep(Protocol.DATA_TIMEOUT / 2);
            }
            catch (Exception ex)
            {

                Logger.WriteMessage($"{title} error read data", ex);
                nwStream = null;

            }
        }

        private void HandlerWrite()
        {

            try
            {
                while (execution)
                {
                    if (DateTime.Now.Second % 10 == 0)
                    {

                        if (nwStream != null)
                        {
                            //ping
                            

                            //send keys
                            nwStream.Write(Protocol.BuildPackage
                                            (Protocol.ConvertObjToByteArr
                                                (encryption.SafetyKeys), Protocol.EPackageTypes.UNENCRYPT));

                            lock (tag_groups)
                            {

                                if (tag_groups.ContainsKey(id))
                                    //send tags
                                    nwStream.Write(Protocol.BuildPackage(encryption.Encrypt(Protocol.ConvertObjToByteArr(tag_groups[id])), Protocol.EPackageTypes.ENCRYPT));
                            }


                        }

                        Thread.Sleep(1000);
                    }

                    Thread.Sleep(Protocol.DATA_TIMEOUT / 2);

                }
            }
            catch (Exception ex)
            {
                Logger.WriteMessage($"{title} error write data", ex);
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
                    byte[] msg = { 7, 7, 7 };
                    nwStream.Write(msg, 0, msg.Length);
                }
                catch (Exception)
                {
                    Logger.WriteMessage($"{title} remote client disconnected");
                    nwStream = null;
                    return;
                }
                Thread.Sleep(1000);
            }

            #endregion

        }
    }
}
