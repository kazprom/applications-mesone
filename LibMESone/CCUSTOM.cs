using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using LibMESone.Tables.Custom;

namespace LibMESone
{
    public class CCUSTOM : CSrvDB
    {



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

                    string name = logger.Name + "_msg_interceptor";

                    if (configuration.FindTargetByName(name) == null)
                    {

                        var target = new NLog.Targets.MethodCallTarget(name, (logEvent, parms) =>
                        {
                            DBLogger.Message(logEvent.TimeStamp, $"[{logEvent.LoggerName}] <{logEvent.Level}> {logEvent.Message} {logEvent.Exception}");
                        });

                        configuration.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target, logger.Name + "*");
                        NLog.LogManager.Configuration = configuration;

                    }
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
