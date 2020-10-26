using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("S7 DB Gate")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: Guid("4913BEB7-4481-417B-8E5E-D1B05DA4FB3A")]

[assembly: AssemblyVersion("1.0.*")]

namespace S7_DB_gate
{
    class Program
    {
        static void Main()
        {
            try
            {

                Lib.Global.PrintAppInfo();
                Lib.Global.Subscribe_Ctrl_C();

                Lib.Console console = new Lib.Console();
                LibMESone.Loggers.TextLogger text_logger = new LibMESone.Loggers.TextLogger();
                LibMESone.ConfigFile config_file = new LibMESone.ConfigFile();

                LibMESone.DBcore core = new LibMESone.DBcore(config_file.DB_TYPE, config_file.CONNECTION_STRING);
               
                LibMESone.Loggers.TextLogCleaner text_log_cleaner = new LibMESone.Loggers.TextLogCleaner(text_logger, config_file.DEPTH_LOG_DAY);
               

                Lib.Global.InfinityWaiting();

            }
            catch (Exception ex)
            {
                Lib.Message.Make("Error main", ex);
                System.Console.ReadKey();
            }
        }
    }
}
