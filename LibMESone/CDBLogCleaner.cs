using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace LibMESone
{
    public class CDBLogCleaner : CSrvCyc
    {

        #region CONSTANTS

        private const string depth_log_day_name = "DEPTH_LOG_DAY";
        private const uint depth_log_days_default = 3;

        #endregion


        private CDBLogger db_logger;

        public CDBLogCleaner(CDBLogger db_logger)
        {
            this.db_logger = db_logger;

            CycleRate = 60000;
        }

        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {

            try
            {

                if (db_logger != null && db_logger.DB != null)
                {

                    uint depth_log_days = depth_log_days_default;

                    CCUSTOM srv_inst = db_logger.Parent as CCUSTOM;

                    if (srv_inst != null && srv_inst.TSettings != null)
                    {
                        Tables.Custom.CSetting setting = srv_inst.TSettings.FirstOrDefault(x => x.Key.Equals(depth_log_day_name, StringComparison.OrdinalIgnoreCase));
                        if (setting != null)
                        {
                            if (!uint.TryParse(setting.Value, out depth_log_days))
                            {
                                Logger.Warn($"Setting [{depth_log_day_name}] can't parse. Default value is {depth_log_days_default}");
                            }
                        }
                        else
                        {
                            Logger.Warn($"Setting [{depth_log_day_name}] not found. Default value is {depth_log_days_default}");
                        }
                    }

                    DateTime ts = new DateTime(DateTime.Now.Ticks - DateTime.Now.Ticks % TimeSpan.TicksPerDay, DateTime.Now.Kind);
                    IEnumerable<string> tables = db_logger.DB.GetListTables(Tables.Custom.CLogMessage.TablePrefix + "%");

                    if (tables != null)
                    {
                        foreach (var table in tables)
                        {
                            if (ts.Subtract(Tables.Custom.CLogMessage.GetTimeStamp(table)).TotalHours > depth_log_days)
                                if (db_logger.DB.RemoveTable(table))
                                    Logger.Info($"Removed log table [{table}]");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                db_logger.Logger.Error(ex);
            }

            base.Timer_Handler(sender, e);
        }

    }
}
