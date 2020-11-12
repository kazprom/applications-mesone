using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace Lib
{
    public class Common
    {

        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private const string layout_info = "[${longdate}] [${level}] ${message:withException=false}";
        private const string layout_error = layout_info + "${newline}${exception}";


        public Common()
        {

            var configuration = new NLog.Config.LoggingConfiguration();
            var consoleInfo = new NLog.Targets.ColoredConsoleTarget("console") { Layout = layout_info };
            var consoleError = new NLog.Targets.ColoredConsoleTarget("console") { Layout = layout_error };

            configuration.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Info, consoleInfo);
            configuration.AddRule(NLog.LogLevel.Warn, NLog.LogLevel.Fatal, consoleError);
            NLog.LogManager.Configuration = configuration;

            logger.Info("Program started!");
            PrintAppInfo();
            Subscribe_Ctrl_C();
        }


        public static string PathExeFolder { get { return System.AppDomain.CurrentDomain.BaseDirectory; } }
        public static string NameExeFile { get { return System.AppDomain.CurrentDomain.FriendlyName; } }



        private string AppName()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.ProductName;
        }

        private string AppInfo()
        {
            return $"{AppName()} (v{AppVersion()}) GUID [{AppGUID()}]";
        }

        private void PrintAppInfo()
        {
            logger.Info(AppInfo());
        }

        private void Subscribe_Ctrl_C()
        {
            try
            {
                System.Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
                {
                    var isCtrlC = e.SpecialKey == ConsoleSpecialKey.ControlC;
                    var isCtrlBreak = e.SpecialKey == ConsoleSpecialKey.ControlBreak;

                    // Prevent CTRL-C from terminating
                    if (isCtrlC)
                    {
                        e.Cancel = true;
                        logger.Info("Program stopped!");
                        System.Environment.Exit(0);
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error waiting Ctrl+C", ex);
            }

        }

        public void InfinityWaiting()
        {
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        public static string AppVersion()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.ProductVersion;
        }

        public static string AppGUID()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            GuidAttribute guidAttr = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true).FirstOrDefault();
            return guidAttr.Value;
        }


    }
}
