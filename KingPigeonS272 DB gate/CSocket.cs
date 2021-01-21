using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;

namespace KingPigeonS272_DB_gate
{
    class CSocket : LibDBgate.CSUB
    {

        #region VARIABLES

        WTcpListener server = null;

        Byte[] buf = new Byte[256];


        #endregion


        #region PROPERTIES

        protected IPAddress ip;
        public dynamic Ip
        {
            get { return ip != null ? ip.ToString() : null; }
            set
            {
                try
                {
                    var _ip = IPAddress.Parse(Convert.ToString(value));

                    if (!Equals(ip, _ip))
                    {
                        ip = _ip;
                        Logger.Info($"IP = {ip}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
            }
        }

        protected ushort? port;
        public dynamic Port
        {
            get { return port; }
            set
            {
                try
                {
                    var _port = ushort.Parse(Convert.ToString(value));

                    if (!Equals(port, _port))
                    {
                        port = _port;
                        Logger.Info($"Port = {port}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
            }
        }


        public dynamic Clients
        {
            set
            {
                try
                {

                    var data = from clients in (IEnumerable<dynamic>)value
                               select new
                               {
                                   Parent = this,
                                   clients.Id,
                                   clients.Name,
                                   clients.Imei,
                                   clients.Timeout_m,
                                   clients.Tags
                               };



                    Dictionary<ulong, Dictionary<string, object>> children_props = data.ToDictionary(o => (ulong)o.Id,
                                                                                                     o => o.
                                                                                                          GetType().
                                                                                                          GetProperties().ToDictionary(z => z.Name,
                                                                                                                                       z => z.GetValue(o)));
                    CUD<CClient>(children_props);


                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        #endregion


        public CSocket()
        {

            try
            {
                CycleRate = 100;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }



        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {

            try
            {

                if (server == null)
                {
                    if (ip != null && port != null)
                    {
                        server = new WTcpListener(ip, (int)port);
                        server.Start();
                        Logger.Info($"Open {ip}:{port}");
                    }
                }
                else
                {

                    if (!Equals(((IPEndPoint)server.LocalEndpoint).Address, ip) || !Equals(((IPEndPoint)server.LocalEndpoint).Port, (int)port))
                    {
                        Logger.Debug($"{((IPEndPoint)server.LocalEndpoint).Address} {ip} {((IPEndPoint)server.LocalEndpoint).Port} {port}");

                        if (server.Active)
                            server.Stop();

                        server = null;
                    }
                }

                if (server != null)
                {

                    try
                    {
                        //Logger.Info("Waiting for a connection");

                        TcpClient client = server.AcceptTcpClient();
                        ThreadPool.QueueUserWorkItem(ClientHandler, client);

                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }

            }
            catch (SocketException s_ex)
            {
                Logger.Warn(s_ex);
                server.Stop();
                server = null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Thread.Sleep(10000);
            }

            base.Timer_Handler(sender, e);

        }

        private void ClientHandler(object obj)
        {

            TcpClient client = obj as TcpClient;

            IPAddress ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
            int port = ((IPEndPoint)client.Client.RemoteEndPoint).Port;

            Logger.Info($"Connected {ip}:{port}");

            NetworkStream stream = client.GetStream();

            int i;

            while ((i = stream.Read(buf, 0, buf.Length)) != 0)
            {
                List<byte> data = new List<byte>();
                data.AddRange(buf.Take(i));

                if (RecognizePackage(ref data))
                {
                    string imei = ParseIMEI(ref data);
                    Logger.Info($"Received package IMEI[{imei}] len={i}");

                    lock (Children)
                    {
                        CClient client_instance = Children.Values.First(x => ((CClient)x).Imei == imei) as CClient;

                        if (client_instance != null)
                        {
                            client_instance.LoadData(data.ToArray());
                        }
                        else
                        {
                            Logger.Warn($"Client with IMEI[{imei}] doesn't registred");
                        }
                    }
                }
            }

            Logger.Info($"Disconnected {ip}:{port}");
            client.Close();
        }


        private string ParseIMEI(ref List<byte> data)
        {
            try
            {
                const byte len = 15;

                if (data != null && data.Count >= len)
                {
                    byte[] imei = data.GetRange(0, len).ToArray();
                    data.RemoveRange(0, len);

                    return Encoding.ASCII.GetString(imei);
                }
                else
                {
                    Logger.Warn("Can't find IMEI in pakage");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;

        }

        private bool RecognizePackage(ref List<byte> data)
        {
            try
            {
                const byte symbol = 0xA5;

                if (data != null &&
                    data.Count > 2 &&
                    data.First() == symbol &&
                    data[1] >= data.Count - 6 &&
                    data[data[1] + 5] == symbol)
                {

                    data.RemoveRange(data[1] + 4, data.Count - (data[1] + 4));
                    data.RemoveRange(0, 8);

                    return true;
                }
                else
                {
                    Logger.Warn($"Bad package [{BitConverter.ToString(data.ToArray())}]");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }



        private class WTcpListener : TcpListener
        {

            public WTcpListener(IPAddress localaddr, int port) : base(localaddr, port)
            {

            }

            public new bool Active { get { return (base.Active); } }

        }

    }
}
