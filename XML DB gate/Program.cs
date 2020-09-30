using System;
using System.Threading;

namespace XML_DB_gate
{
    class Program
    {
        public static string PathExeFolder { get { return System.AppDomain.CurrentDomain.BaseDirectory; } }
        public static string NameExeFile { get { return System.AppDomain.CurrentDomain.FriendlyName; } }

        static void Main(string[] args)
        {
            PrintInfo();
            Config config = Config.GetInstance();
            DataBase dataBase = DataBase.GetInstance();

            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
            {

                var isCtrlC = e.SpecialKey == ConsoleSpecialKey.ControlC;
                var isCtrlBreak = e.SpecialKey == ConsoleSpecialKey.ControlBreak;

                // Prevent CTRL-C from terminating
                if (isCtrlC)
                {
                    e.Cancel = true;
                    Console.WriteLine("Programm stopped!");
                    System.Environment.Exit(0);
                }
            };

            DateTime ts_start = DateTime.Now;

            while (true)
            {
                ts_start = DateTime.Now;
                Thread.Sleep(60000);
            }
        }

        private static void PrintInfo()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            var productName = fvi.ProductName;
            var productVersion = fvi.ProductVersion;
            Console.WriteLine(productName + $"(v{productVersion})");
        }
    }
}
