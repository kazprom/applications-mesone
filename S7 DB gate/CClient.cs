using LibMESone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Timers;

namespace S7_DB_gate
{
    public class CClient : LibPlcDBgate.CClient
    {

        #region VARIABLES

        public S7.Net.Plc plc { get; set; }

        #endregion

        #region PROPERTIES

        private S7.Net.CpuType cpu_type;
        public dynamic Cpu_type
        {
            get { return cpu_type.ToString(); }
            set
            {
                try
                {
                    cpu_type = Enum.Parse(typeof(S7.Net.CpuType), value);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
            }
        }

        private ushort rack;
        public dynamic Rack
        {
            get { return rack; }
            set
            {
                try
                {
                    rack = ushort.Parse(Convert.ToString(value));
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
            }
        }

        private ushort slot;
        public dynamic Slot
        {
            get { return slot; }
            set
            {
                try
                {
                    slot = ushort.Parse(Convert.ToString(value));
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
            }
        }


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

                                          S7_Data_Type = group_tags.PLC_data_type,
                                          DB = group_tags.Data_block_no,
                                          StartByteAdr = group_tags.Data_block_offset,
                                          BitAdr = group_tags.Bit_offset,
                                          S7_Var_Type = group_tags.Request_type
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
                    if (plc != null && plc.IsConnected)
                    {
                        plc.Close();
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
                if (plc == null)
                {
                    if (ip != null && port != null)
                        plc = new S7.Net.Plc(cpu_type, ip.ToString(), (int)port, (short)rack, (short)slot);
                    else
                    {
                        Logger.Warn($"Incorrect connection settings");
                    }

                }

                if (plc != null)
                {
                    lock (plc)
                    {

                        if (plc.CPU != cpu_type || plc.IP != ip.ToString() || plc.Port != port || plc.Rack != rack || plc.Slot != slot)
                        {
                            if (plc.IsConnected)
                            {
                                plc.Close();
                                Logger.Info($"Closed connection");
                            }

                            plc = new S7.Net.Plc(cpu_type, ip.ToString(), (int)port, (short)rack, (short)slot);

                        }

                        if (!plc.IsConnected)
                        {
                            try
                            {
                                plc.Open();
                                Logger.Info($"Openned connection");
                            }
                            catch (Exception ex)
                            {
                                Logger.Warn(ex, $"Open connection");
                            }
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
