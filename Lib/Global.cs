using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Lib
{
    public class Global
    {

        #region PUBLICS
        public static string PathExeFolder { get { return System.AppDomain.CurrentDomain.BaseDirectory; } }
        public static string NameExeFile { get { return System.AppDomain.CurrentDomain.FriendlyName; } }

        public static void Wait_Ctrl_C()
        {
            try
            {
                Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
                            {
                                var isCtrlC = e.SpecialKey == ConsoleSpecialKey.ControlC;
                                var isCtrlBreak = e.SpecialKey == ConsoleSpecialKey.ControlBreak;

                // Prevent CTRL-C from terminating
                if (isCtrlC)
                                {
                                    e.Cancel = true;
                                    Console.WriteLine("Program stopped!");
                                    System.Environment.Exit(0);
                                }
                            };


                while (true)
                {
                    Thread.Sleep(60000);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error waiting Ctrl+C", ex);
            }

        }

        public static void PrintInfo()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var productName = fvi.ProductName;
            var productVersion = fvi.ProductVersion;
            Console.WriteLine(productName + $" (v{productVersion})");
        }

        #endregion



    }
}
