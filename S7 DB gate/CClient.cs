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

        #region CONSTANTS

#if DEBUG
        private const int period = 5000;
#else
        private const int period = 30000;
#endif

        #endregion

        #region VARIABLES

        public S7.Net.Plc PLC { get; set; }

        #endregion

        #region PROPERTIES

        public Structs.CClient Settings { get; set; }


        #endregion


        #region DESTRUCTOR

        public override void Dispose(bool disposing)
        {

            if (!disposedValue)
            {
                if (disposing)
                {
                    if (PLC != null && PLC.IsConnected)
                    {
                        PLC.Close();
                        Logger.Info($"Closed connection");
                    }
                }
            }

            base.Dispose(disposing);

        }

        #endregion

        #region PUBLICS


        public override void LoadSetting(ISetting setting)
        {

            try
            {
                Settings = setting as Structs.CClient;

                CUD<CGroup>(Settings.Groups);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            base.LoadSetting(setting);

        }



        /*
        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {

            try
            {

                //-----------read------------------

                IEnumerable<Structs.Tag> tags = null;
                if (Parent.Database.CompareTableSchema<Structs.Tag>(Structs.Tag.TableName))
                    tags = Parent.Database.WhereRead<Structs.Tag>(Structs.Tag.TableName, new { Enabled = true, Clients_id = ID });

                if (tags != null)
                {

                    IEnumerable<ushort> fresh_rates = tags.GroupBy(x => x.Rate).Select(x => x.First()).Select(x => x.Rate);
                    IEnumerable<ushort> existing_rates = this.Groups.Keys;

                    IEnumerable<ushort> waste = existing_rates.Except(fresh_rates);
                    IEnumerable<ushort> modify = fresh_rates.Intersect(existing_rates);
                    IEnumerable<ushort> missing = fresh_rates.Except(existing_rates);

                    foreach (ushort rate in waste)
                    {
                        Groups[rate].Dispose();
                        Groups.Remove(rate);
                    }

                    foreach (ushort rate in modify)
                    {
                        Groups[rate].LoadTags(tags.Where(x => x.Rate == rate));
                    }

                    foreach (ushort rate in missing)
                    {
                        CGroup group = new CGroup(this, rate);
                        group.LoadTags(tags.Where(x => x.Rate == rate));
                        Groups.Add(rate, group);
                    }

                }
                else
                {
                    foreach (CGroup group in Groups.Values)
                    {
                        group.Dispose();
                    }
                    Groups.Clear();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Title}. DB Handler");
            }

            base.Timer_Handler(sender, e);
        }
        */
        #endregion

        #region PRIVATES

        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {

            try
            {
                if (PLC == null)
                {
                    IPAddress ip_result;
                    if (IPAddress.TryParse(Settings.Ip, out ip_result) && Settings.Port != 0)
                        PLC = new S7.Net.Plc(Settings.Cpu_type, IP, (int)Port, (short)Rack, (short)Slot);
                    else
                    {
                        Logger.Warn($"Incorrect connection settings");
                    }

                }

                if (PLC != null)
                {
                    lock (PLC)
                    {

                        if (PLC.CPU != CPU_type || PLC.IP != IP || PLC.Port != Port || PLC.Rack != Rack || PLC.Slot != Slot)
                        {
                            if (PLC.IsConnected)
                            {
                                PLC.Close();
                                Logger.Info($"Closed connection");
                            }

                            PLC = new S7.Net.Plc(CPU_type, IP, (int)Port, (short)Rack, (short)Slot);

                        }

                        if (!PLC.IsConnected)
                        {
                            try
                            {
                                PLC.Open();
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
