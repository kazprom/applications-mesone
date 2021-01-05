using LibMESone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace LibPlcDBgate
{
    public class CHistoryCleaner : CSrvCyc
    {

        #region CONSTANTS

        private const string depth_history_hour_name = "DEPTH_HISTORY_HOUR";
        private const uint depth_history_hour_default = 24;

        #endregion


        private CHistorian historian;

        public CHistoryCleaner(CHistorian historian)
        {
            this.historian = historian;
            CycleRate = 60000;
        }

        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {

            try
            {

                if (historian != null && historian.DB != null)
                {

                    uint depth_history_hours = depth_history_hour_default;

                    CCUSTOM srv_inst = historian.Parent as CCUSTOM;

                    if (srv_inst != null && srv_inst.TSettings != null)
                    {
                        LibMESone.Tables.Custom.CSetting setting = srv_inst.TSettings.FirstOrDefault(x => x.Key.Equals(depth_history_hour_name, StringComparison.OrdinalIgnoreCase));
                        if (setting != null)
                        {
                            if (!uint.TryParse(setting.Value, out depth_history_hours))
                            {
                                Logger.Warn($"Setting [{depth_history_hour_name}] can't parse. Default value is {depth_history_hour_default}");
                            }
                        }
                        else
                        {
                            Logger.Warn($"Setting [{depth_history_hour_name}] not found. Default value is {depth_history_hour_default}");
                        }
                    }

                    DateTime ts = new DateTime(DateTime.Now.Ticks - DateTime.Now.Ticks % TimeSpan.TicksPerHour, DateTime.Now.Kind);
                    IEnumerable<string> tables = historian.DB.GetListTables(Tables.CHisValue.TablePrefix + "%");

                    if (tables != null)
                    {
                        foreach (var table in tables)
                        {
                            if (ts.Subtract(Tables.CHisValue.GetTimeStamp(table)).TotalHours > depth_history_hours)
                                historian.DB.RemoveTable(table);
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

    }
}
