using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibMESone.Structs;
using LibMESone.Tables.Custom;

namespace LibMESone
{
    public class CSrvCUSTOM : CCyclycService
    {

        #region CONSTANTS

#if DEBUG
        private const int period = 5000;
#else
        private const int period = 60000;
#endif


        private const string depth_log_day_name = "DEPTH_LOG_DAY";
        private const uint depth_log_days_default = 3;

        #endregion

        #region VARIABLES

        private Lib.Buffer<CLogMessage> log_buf;
        private DateTime log_ts = default;
        private string logger_name;

        #endregion

        #region PROPERTIES

        public Lib.Database Database { get; set; }

        public IEnumerable<Tables.Custom.CSetting> Settings { get; private set; }

        #endregion

        #region CONSTRUCTOR
        public CSrvCUSTOM()
        {

            log_buf = new Lib.Buffer<CLogMessage>(100, 5000);
            log_buf.CyclicEvent += LogDataHandler;
            log_buf.HalfEvent += LogDataHandler;

            CycleRate = period;

        }

        #endregion

        #region DESTRUCTOR


        public override void Dispose(bool disposing)
        {

            if (!disposedValue)
            {
                if (disposing)
                {

                    base.Dispose(disposing);

                    LogDataHandler();

                    var configuration = NLog.LogManager.Configuration;
                    configuration.RemoveRuleByName(logger_name);
                    NLog.LogManager.Configuration = configuration;

                    Database = null;
                }
            }

        }


        #endregion

        #region PUBLICS


        public override void LoadSetting(ISetting setting)
        {

            try
            {

                CSetCUSTOM _setting = setting as CSetCUSTOM;

                ID = _setting.Id;
                Name = _setting.Name;

                logger_name = $"Proc [{ID}] <{Name}>";

                if (Logger != NLog.LogManager.GetLogger(logger_name))
                {
                    if (Logger == null)
                    {
                        var configuration = NLog.LogManager.Configuration;
                        var target = new NLog.Targets.MethodCallTarget(logger_name, (logEvent, parms) =>
                        {
                            log_buf.Enqueue(new CLogMessage() { Timestamp = logEvent.TimeStamp, Message = $"{logEvent.Level} {logEvent.Message} {logEvent.Exception}" });
                        });


                        configuration.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target, logger_name + "*");
                        NLog.LogManager.Configuration = configuration;

                        Logger = NLog.LogManager.GetLogger(logger_name);
                    }
                    else
                    {

                    }
                }





                if (Database == null)
                    Database = new Lib.Database(ID, Logger);

                Database.LoadSettings(_setting.Driver,
                                      _setting.Host,
                                      _setting.Port,
                                      _setting.Charset,
                                      _setting.Database,
                                      _setting.Username,
                                      _setting.Password);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public override void Timer_Handler(object state)
        {
            try
            {

                if (Database != null)
                {

                    //--------read----------------

                    Settings = null;

                    switch (Database.CheckExistTable(Tables.Custom.CSetting.TableName))
                    {

                        case null:
                        case false:
                            return;
                    }

                    switch (Database.CompareTableSchema<Tables.Custom.CSetting>(Tables.Custom.CSetting.TableName))
                    {
                        case null:
                        case false:
                            return;
                    }

                    Settings = Database.Read<Tables.Custom.CSetting>(Tables.Custom.CSetting.TableName);


                    //------------log cleaner-----------

                    uint depth_log_days = depth_log_days_default;
                    if (Settings != null)
                    {
                        Tables.Custom.CSetting setting = Settings.FirstOrDefault(x => x.Key.Equals(depth_log_day_name, StringComparison.OrdinalIgnoreCase));
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


                    IEnumerable<string> tables = Database.GetListTables(Tables.Custom.CLogMessage.TablePrefix + "%");
                    DateTime ts = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                    if (tables != null)
                    {
                        foreach (string table in tables)
                        {
                            if (ts.Subtract(CLogMessage.GetTimeStamp(table)).TotalDays > depth_log_days)
                            {
                                if (Database.RemoveTable(table))
                                    Logger.Info($"Removed log table [{table}]");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Logger.Error(ex);
            }

        }



        #endregion

        #region PRIVATES

        private async void LogDataHandler()
        {
            await Task.Run(() =>
            {

                try
                {

                    CLogMessage data = null;

                    if (Database != null)
                    {
                        while (log_buf.Count > 0)
                        {

                            if (data == null)
                            {
                                data = log_buf.Dequeue();


                                if (log_ts == default || log_ts != data.Timestamp)
                                {

                                    switch (Database.CheckExistTable(CLogMessage.GetTableName(data.Timestamp)))
                                    {
                                        case false:
                                            if (Database.CreateTable<CLogMessage>(CLogMessage.GetTableName(data.Timestamp)))
                                            {
                                                log_ts = data.Timestamp;
                                            }
                                            break;
                                    }

                                }

                                if (Database.Insert(CLogMessage.GetTableName(data.Timestamp), data))
                                {
                                    data = null;
                                }
                                else
                                {
                                    Thread.Sleep(1000);
                                }
                            }
                        }
                    }
                }
                finally
                {

                }
            });
        }










        #endregion

    }
}
