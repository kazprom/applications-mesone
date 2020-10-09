using System.Reflection;
using System.Threading;

[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("OPC DB Gate Server")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("1.0.*")]


namespace OPC_DB_gate_server
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

                Lib.Buffer<OPC_DB_gate_Lib.TagData> buffer = new Lib.Buffer<OPC_DB_gate_Lib.TagData>(10000);

                Settings settings = new Settings();
                Clients clients = new Clients();
                Tags tags = new Tags();
                RT_values rt_values = new RT_values();
                History history = new History();
                Application application = new Application(); application.Put(Application.EKeys.APPINFO, Lib.Global.AppInfo());
                Diagnostics diagnostics = new Diagnostics();

                HandlerDatabase database = new HandlerDatabase(config_file.DB_TYPE,
                                                               config_file.CONNECTION_STRING,
                                                               settings, clients, tags,
                                                               rt_values, history, application, diagnostics);

                Lib.DBLogger db_logger = new Lib.DBLogger(database.Database);

                HandlerData data = new HandlerData(buffer,
                                                   rt_values, settings.RT_VALUES_ENABLE,
                                                   history, settings.HISTORY_ENABLE,
                                                   application);


                Connections connections = new Connections(clients, tags, buffer, diagnostics);



                HistoryCleaner history_cleaner = new HistoryCleaner(database.Database, settings.DEPTH_HISTORY_HOUR);
                Lib.TextLogCleaner text_log_cleaner = new Lib.TextLogCleaner(text_logger, config_file.DEPTH_LOG_DAY);
                Lib.DBLogCleaner db_log_cleaner = new Lib.DBLogCleaner(db_logger, settings.DEPTH_LOG_DAY);


                while (true)
                {
                    Thread.Sleep(100);
                }

            }
            catch (System.Exception ex)
            {
                Lib.Message.Make("Error main", ex);
                System.Console.ReadKey();
            }

        }
    }
}
