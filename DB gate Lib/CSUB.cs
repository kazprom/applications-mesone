
using Lib;
using System;
using System.Diagnostics;

namespace LibDBgate
{
    public abstract class CSUB : LibMESone.CSrvCyc
    {

        #region PROPERTIES

        public Tables.CDiagnostic Diagnostic { get; set; }

        #endregion

        public override CParent Parent
        {
            get => base.Parent;
            set
            {
                try
                {

                    if (!Equals(base.Parent, value))
                    {

                        base.Parent = value;
                        Lib.CParent parent = this.Parent;
                        while (parent != null)
                        {
                            if (parent is CCUSTOM)
                            {
                                CCUSTOM custom = parent as CCUSTOM;
                                custom.Diagnoster.Subscribe(this);
                            }

                            parent = parent.Parent;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

            }
        }



        public CSUB():base()
        {
            LoggerMaked += CSUB_LoggerMaked;
        }

        private void CSUB_LoggerMaked(NLog.Logger logger)
        {

            try
            {

                if (logger != null)
                {
                    var configuration = NLog.LogManager.Configuration;

                    string name = logger.Name + "_msg_interceptor";

                    if (configuration.FindTargetByName(name) == null)
                    {

                        var target = new NLog.Targets.MethodCallTarget(name, DiagnosticMessage);

                        configuration.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target, logger.Name);
                        NLog.LogManager.Configuration = configuration;

                    }
                }
            }
            catch (Exception ex)
            {
                if (Logger != null) Logger.Error(ex);
            }

        }


        private void DiagnosticMessage(NLog.LogEventInfo logEvent, object[] parms)
        {
            try
            {
                if (Diagnostic != null)
                {
                    Diagnostic.State = logEvent.Level.ToString();
                    Diagnostic.Message = $"{logEvent.Message}";
                    if(logEvent.Exception != null)
                        Diagnostic.Message += $"\n{logEvent.Exception.Message}";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


        }
    }
}
