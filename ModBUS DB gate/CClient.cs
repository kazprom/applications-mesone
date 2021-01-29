using Modbus.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Timers;

namespace ModBUS_DB_gate
{
    class CClient : LibPlcDBgate.CClient
    {

        #region VARIABLES

        private dynamic client;

        #endregion


        #region ENUMS

        public enum EProtocol
        {
            TCP,
            UDP
        }

        #endregion


        #region PROPERTIES

        public ModbusIpMaster Plc;

        private EProtocol protocol;
        public dynamic Protocol
        {
            get { return protocol.ToString(); }
            set
            {
                try
                {
                    protocol = Enum.Parse(typeof(EProtocol), value, true);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
            }
        }

        private byte address;
        public dynamic Address
        {
            get { return address; }
            set
            {
                try
                {
                    address = byte.Parse(Convert.ToString(value));
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
            }
        }


        public bool Sb { get; set; }

        public bool Sw { get; set; }


        public override dynamic Tags
        {
            set
            {

                var data = from tags in (IEnumerable<dynamic>)value
                           group tags by tags.Rate into groups
                           select new
                           {
                               Parent = this,
                               Id = groups.Key,

                               Tags = from group_tags in groups
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
                CUD<CGroup>(children_props);
            }
        }

        #endregion

        #region DESTRUCTOR

        public override void Dispose(bool disposing)
        {

            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Plc != null)
                    {
                        Plc.Dispose();
                        Logger.Info($"Closed connection");
                    }
                }
            }

            base.Dispose(disposing);

        }

        #endregion

        #region PRIVATES

        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {

            try
            {

                if (client == null)
                {
                    try
                    {
                        switch (protocol)
                        {
                            case EProtocol.TCP:
                                client = new TcpClient(Ip, Port);
                                break;
                            case EProtocol.UDP:
                                client = new UdpClient(Ip, Port);
                                break;
                        }

                        Logger.Info("Openned connection");
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "Open connection");
                    }
                }

                if (client != null)
                {

                    if (Plc == null)
                    {
                        try
                        {
                            Plc = ModbusIpMaster.CreateIp(client);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, "Driver");
                        }

                    }
                    else
                    {
                        if (!client.Connected)
                        {
                            Plc.Dispose();
                            Plc = null;
                            client.Dispose();
                            client = null;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            base.Timer_Handler(sender, e);
        }




        #endregion

    }
}
