using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace Lib
{
    public class Global
    {

        #region PUBLICS
        public static string PathExeFolder { get { return System.AppDomain.CurrentDomain.BaseDirectory; } }
        public static string NameExeFile { get { return System.AppDomain.CurrentDomain.FriendlyName; } }

        public static void Subscribe_Ctrl_C()
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
                                    System.Console.WriteLine("Program stopped!");
                                    System.Environment.Exit(0);
                                }
                            };
            }
            catch (Exception ex)
            {
                throw new Exception("Error waiting Ctrl+C", ex);
            }

        }

        public static string AppName()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.ProductName;
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

        public static string AppInfo()
        {
            return $"{AppName()} (v{AppVersion()}) GUID [{AppGUID()}]";
        }

        public static void PrintAppInfo()
        {
            System.Console.WriteLine(AppInfo());
        }

        public static void InfinityWaiting()
        {
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        #endregion



    }
}
