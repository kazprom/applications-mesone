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
        private Lib.Encryption encryption = new Lib.Encryption(true);
        private Lib.Protocol protocol = new Lib.Protocol();
        byte[] buf = new byte[Lib.Protocol.SIZE_BUFFER];
        private Dictionary<int, OPC_DB_gate_Lib.TagSettings> tags = new Dictionary<int, OPC_DB_gate_Lib.TagSettings>();
        private Lib.Buffer<OPC_DB_gate_Lib.TagData> buffer;
        #endregion

        #region PROPERTIES

        private IPAddress ip;
        public IPAddress IP { get { return ip; } }

        private int port;
        public int Port { get { return port; } }

        #endregion

        #region CONSTRUCTOR

        public TCPconnection(Lib.Buffer<OPC_DB_gate_Lib.TagData> buffer)
        {
            this.buffer = buffer;
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

                    Lib.Message.Make($"{title} changed settings to {ip}:{port}. Reconnecting ...");
                }

                GC.Collect();

                this.port = port;
                this.ip = ip;

                title = $"Listener {ip}:{port}";


                execution = true;

                thread_connect = new Thread(new ThreadStart(HadlerConnect)) { IsBackground = true, Name = $"Connect Ip - {this.ip}:{this.port}" };
                thread_connect.Start();

            }
        }

        public void PutTag(int id, OPC_DB_gate_Lib.TagSettings tag)
        {
            try
            {
                lock (tags)
                {
                    if (!tags.ContainsKey(id))
                        tags.Add(id, new OPC_DB_gate_Lib.TagSettings());
                    tags[id] = tag;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error put tag", ex);
            }
        }

        public void RemoveTag(int id)
        {
            try
            {
                lock (tags)
                {
                    if (tags.ContainsKey(id))
                        tags.Remove(id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error remove tag", ex);
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
                        Lib.Message.Make($"{title} opened");

                        using (client = new TcpClient())
                        {
                            while (execution)
                            {
                                try
                                {

                                    if (!client.Connected)
                                    {
                                        Lib.Message.Make($"{title} waiting ...");
                                        if (listener.Pending())
                                            client = listener.AcceptTcpClient();
                                    }
                                    else
                                    {
                                        if (nwStream == null)
                                        {
                                            nwStream = client.GetStream();
                                            Lib.Message.Make($"{title} connected with client {((IPEndPoint)client.Client.RemoteEndPoint).Address}:{((IPEndPoint)client.Client.RemoteEndPoint).Port}");

                                            thread_read = new Thread(new ThreadStart(HandlerRead)) { IsBackground = true, Name = $"Read Ip - {this.ip}:{this.port}" };
                                            thread_read.Start();

                                            thread_write = new Thread(new ThreadStart(HandlerWrite)) { IsBackground = true, Name = $"Write Ip - {this.ip}:{this.port}" };
                                            thread_write.Start();

                                            thread_ping = new Thread(new ThreadStart(HandlerPing)) { IsBackground = true, Name = $"Ping Ip - {this.ip}:{this.port}" };
                                            thread_ping.Start();
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Lib.Message.Make($"{title} error connection", ex);
                                }

                                Thread.Sleep(5000);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Lib.Message.Make($"{title} error opening", ex);
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

                Lib.Message.Make($"{title} closed");
            }
            catch (Exception ex)
            {
                Lib.Message.Make($"{title} error closing", ex);
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

                                    switch (Lib.Protocol.GetTypePackage(ref data))
                                    {
                                        case Lib.Protocol.EPackageTypes.ENCRYPT:
                                            try
                                            {
                                                object obj = Lib.Protocol.ConvertByteArrToObj(encryption.Decrypt(data));

                                                if (obj.GetType().Equals(typeof(OPC_DB_gate_Lib.TagData)))
                                                {
                                                    buffer.Enqueue((OPC_DB_gate_Lib.TagData)obj);
                                                }


                                            }
                                            catch (Exception ex)
                                            {
                                                Lib.Message.Make($"{title} error getting package with data object", ex);
                                            }
                                            break;
                                    }
                                }
                            }

                        }
                    }
                }

                Thread.Sleep(Lib.Protocol.DATA_TIMEOUT / 2);
            }
            catch (Exception ex)
            {

                Lib.Message.Make($"{title} error read data", ex);
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
                            nwStream.Write(Lib.Protocol.BuildPackage
                                            (Lib.Protocol.ConvertObjToByteArr
                                                (encryption.SafetyKeys), Lib.Protocol.EPackageTypes.UNENCRYPT));

                            lock (tags)
                            {
                                //send tags
                                nwStream.Write(Lib.Protocol.BuildPackage(encryption.Encrypt(Lib.Protocol.ConvertObjToByteArr(tags)), Lib.Protocol.EPackageTypes.ENCRYPT));
                            }


                        }

                        Thread.Sleep(1000);
                    }

                    Thread.Sleep(Lib.Protocol.DATA_TIMEOUT / 2);

                }
            }
            catch (Exception ex)
            {
                Lib.Message.Make($"{title} error write data", ex);
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
                    Lib.Message.Make($"{title} remote client disconnected");
                    nwStream = null;
                    return;
                }
                Thread.Sleep(1000);
            }

            #endregion

        }
    }
}
