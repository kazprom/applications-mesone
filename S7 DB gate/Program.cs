using System;
using System.Reflection;
using System.Threading;

[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("S7 DB Gate")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("1.0.*")]

namespace S7_DB_gate
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                Lib.Global.PrintAppInfo();
                Lib.Global.Subscribe_Ctrl_C();

                Lib.Console console = new Lib.Console();
                Lib.TextLogger text_logger = new Lib.TextLogger($@"{Lib.Global.PathExeFolder}LOG");

                HandlerConfigFile config_file;

                if (args.Length == 1)
                {
                    config_file = new HandlerConfigFile(args[0]);
                }
                else
                {
                    config_file = new HandlerConfigFile(Lib.Global.NameExeFile.Split('.')[0] + ".xml");
                }

                Lib.Buffer<DB_gate_Lib.TagData> buffer = new Lib.Buffer<DB_gate_Lib.TagData>(10000);

                Settings settings = new Settings();
                Clients clients = new Clients();
                Tags tags = new Tags();
                DB_gate_Lib.RT_values rt_values = new DB_gate_Lib.RT_values();
                DB_gate_Lib.History history = new DB_gate_Lib.History();
                Lib.Application application = new Lib.Application(); application.Put(Lib.Application.EKeys.APPINFO, Lib.Global.AppInfo());
                Diagnostics diagnostics = new Diagnostics();

                HandlerDatabase database = new HandlerDatabase(config_file.DB_TYPE,
                                                               config_file.CONNECTION_STRING,
                                                               settings, clients, tags,
                                                               rt_values, history, application, diagnostics);
                
                Lib.DBLogger db_logger = new Lib.DBLogger(database.Database);


                Connections connections = new Connections(clients, tags, buffer, diagnostics);

                DB_gate_Lib.HistoryCleaner history_cleaner = new DB_gate_Lib.HistoryCleaner(database.Database, settings.DEPTH_HISTORY_HOUR);
                Lib.TextLogCleaner text_log_cleaner = new Lib.TextLogCleaner(text_logger, config_file.DEPTH_LOG_DAY);
                Lib.DBLogCleaner db_log_cleaner = new Lib.DBLogCleaner(db_logger, settings.DEPTH_LOG_DAY);

                while (true)
                {
                    Thread.Sleep(1000);
                }


            }
            catch (Exception ex)
            {
                Lib.Message.Make("Error main", ex);
                System.Console.ReadKey();
            }
        }
    }
}
