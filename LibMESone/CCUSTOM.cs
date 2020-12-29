using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using LibMESone.Tables.Custom;

namespace LibMESone
{
    public class CCUSTOM : CSrvDB
    {

        #region CONSTANTS

        private const string depth_log_day_name = "DEPTH_LOG_DAY";
        private const uint depth_log_days_default = 3;

        #endregion

        #region VARIABLES


        #endregion

        #region PROPERTIES

        public CDBLogger DBLogger { get; set; } = new CDBLogger();


        public IEnumerable<Tables.Custom.CSetting> TSettings { get; private set; }

        #endregion

        #region CONSTRUCTOR
        public CCUSTOM()
        {

            DBLogger.DB = DB;

            LoggerMaked += CCUSTOM_LoggerMaked;

        }

        private void CCUSTOM_LoggerMaked(NLog.Logger logger)
        {
            try
            {

                if (logger != null)
                {
                    var configuration = NLog.LogManager.Configuration;
                    var target = new NLog.Targets.MethodCallTarget(logger.Name, (logEvent, parms) =>
                    {
                        DBLogger.Message(logEvent.TimeStamp, $"{logEvent.Level} {logEvent.Message} {logEvent.Exception}");
                    });


                    configuration.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target, logger.Name + "*");
                    NLog.LogManager.Configuration = configuration;

                }

            }
            catch (Exception ex)
            {
                if (Logger != null) Logger.Error(ex);
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
                    var configuration = NLog.LogManager.Configuration;
                    configuration.RemoveTarget(Logger.Name);
                    configuration.RemoveRuleByName(Logger.Name + "*");
                    NLog.LogManager.Configuration = configuration;
                }
            }

            base.Dispose(disposing);

        }

        #endregion

        #region PUBLICS

        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (DBprops != null)
                {
                    DB.LoadSettings(DBprops);
                }

                if (DB != null)
                {

                    //--------read----------------

                    TSettings = null;

                    switch (DB.CheckExistTable(Tables.Custom.CSetting.TableName))
                    {

                        case true:
                            switch (DB.CompareTableSchema<Tables.Custom.CSetting>(Tables.Custom.CSetting.TableName))
                            {
                                case true:
                                    TSettings = DB.Read<Tables.Custom.CSetting>(Tables.Custom.CSetting.TableName);
                                    break;
                            }
                            break;
                    }




                    //------------log cleaner-----------

                    uint depth_log_days = depth_log_days_default;
                    if (TSettings != null)
                    {
                        Tables.Custom.CSetting setting = TSettings.FirstOrDefault(x => x.Key.Equals(depth_log_day_name, StringComparison.OrdinalIgnoreCase));
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


                    IEnumerable<string> tables = DB.GetListTables(Tables.Custom.CLogMessage.TablePrefix + "%");
                    DateTime ts = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                    if (tables != null)
                    {
                        foreach (string table in tables)
                        {
                            if (ts.Subtract(CLogMessage.GetTimeStamp(table)).TotalDays > depth_log_days)
                            {
                                if (DB.RemoveTable(table))
                                    Logger.Info($"Removed log table [{table}]");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                if (Logger != null) Logger.Error(ex);
            }

            base.Timer_Handler(sender, e);

        }

        #endregion

        
    }
}
