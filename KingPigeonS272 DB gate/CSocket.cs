using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;

namespace KingPigeonS272_DB_gate
{
    class CSocket : LibDBgate.CSUB
    {

        #region VARIABLES

        TcpListener server = null;
        TcpClient client;

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
                    ip = IPAddress.Parse(Convert.ToString(value));
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
                    port = ushort.Parse(Convert.ToString(value));
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

                    var data = from tags in (IEnumerable<dynamic>)value
                               group tags by tags.Rate into groups
                               select new
                               {
                                   Parent = this,
                                   Id = groups.Key,

                                   Groups = from group_tags in groups
                                            select new
                                            {
                                                group_tags.Id,
                                                group_tags.Name,

                                                group_tags.Data_type,
                                                group_tags.History_enabled,
                                                group_tags.RT_values_enabled,

                                                group_tags.Function,
                                                group_tags.Address,
                                            }
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
                CycleRate = 10;
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
                        server = new TcpListener(ip, (ushort)port);
                        server.Start();
                        client = server.AcceptTcpClient();
                    }
                }
                else
                {
                    //if(server.LocalEndpoint.AddressFamily != ip &&)
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            base.Timer_Handler(sender, e);

        }


    }
}
