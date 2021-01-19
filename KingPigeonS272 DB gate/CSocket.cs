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
                        server = new WTcpListener(ip, (int)port);
                }
                else
                {
                    if (((IPEndPoint)server.LocalEndpoint).Address != ip || ((IPEndPoint)server.LocalEndpoint).Port != port)
                    {
                        if (server.Server.Connected)
                            server.Stop();

                        server = null;
                    }
                }

                if (server != null)
                {
                    if (!server.Active)
                    {
                        server.Start();
                        Logger.Info("Open");
                    }

                    if (server.Active)
                    {

                        try
                        {
                            Logger.Info("Waiting for a connection");

                            TcpClient client = server.AcceptTcpClient();

                            Logger.Info($"Connected to {((IPEndPoint)client.Client.RemoteEndPoint).Address}:{((IPEndPoint)client.Client.RemoteEndPoint).Port}");

                            NetworkStream stream = client.GetStream();

                            int i;
                            //String data = null;

                            while ((i = stream.Read(buf, 0, buf.Length)) != 0)
                            {
                                //data = System.Text.Encoding.ASCII.GetString(buf, 0, i);
                                Logger.Info($"receive package: {BitConverter.ToString(buf, 0, i)}");
                            }



                            client.Close();

                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }

                    }

                }




            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Thread.Sleep(10000);
            }

            base.Timer_Handler(sender, e);

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
