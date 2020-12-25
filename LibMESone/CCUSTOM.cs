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

        #endregion

        #region PROPERTIES

        

        public IEnumerable<Tables.Custom.CSetting> Settings { get; private set; }



        public string Driver { get; set; }

        public string Host { get; set; }

        public uint Port { get; set; }

        public string Charset { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }


        #endregion

        #region CONSTRUCTOR
        public CCUSTOM()
        {

            log_buf = new Lib.Buffer<CLogMessage>(100, 5000);
            log_buf.CyclicEvent += LogDataHandler;
            log_buf.HalfEvent += LogDataHandler;


            CycleRate = period;

        }

        private void CSrvCustom_PropsChangedEvent(CChildProps props)
        {
            try
            {

                if (Logger != null && Logger != NLog.LogManager.GetLogger(logger_name))
                {

                    var configuration = NLog.LogManager.Configuration;
                    configuration.RemoveTarget(logger_name);
                    configuration.RemoveRuleByName(logger_name + "*");
                    NLog.LogManager.Configuration = configuration;
                    Logger = null;

                }

                if (Logger == null)
                {
                    var configuration = NLog.LogManager.Configuration;
                    var target = new NLog.Targets.MethodCallTarget(Logger.Name, (logEvent, parms) =>
                    {
                        log_buf.Enqueue(new CLogMessage() { Timestamp = logEvent.TimeStamp, Message = $"{logEvent.Level} {logEvent.Message} {logEvent.Exception}" });
                    });


                    configuration.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target, Logger.Name + "*");
                    NLog.LogManager.Configuration = configuration;

                }

                if (Database == null)
                    Database = new Lib.CDatabase(Props.Id, Logger);

                CSrvCustomProps set = Props as CSrvCustomProps;

                Database.LoadSettings(set.Driver,
                                      set.Host,
                                      set.Port,
                                      set.Charset,
                                      set.Database,
                                      set.Username,
                                      set.Password);

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


                    LogDataHandler();

                    var configuration = NLog.LogManager.Configuration;
                    configuration.RemoveRuleByName(Logger.Name);
                    NLog.LogManager.Configuration = configuration;

                    Database = null;

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

                if (Logger != null) Logger.Error(ex);
            }

            base.Timer_Handler(sender, e);

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
