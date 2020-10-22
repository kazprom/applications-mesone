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

                Lib.Buffer<LibDBgate.TagData> buffer = new Lib.Buffer<LibDBgate.TagData>(10000);

                Tables.Tsettings settings = new Tables.Tsettings();
                Tables.Tclients clients = new Tables.Tclients();
                Tables.Ttags tags = new Tables.Ttags();
                Tables.Tdiagnostics diagnostics = new Tables.Tdiagnostics();

                LibDBgate.Trt_values rt_values = new LibDBgate.Trt_values();
                LibDBgate.HistoryFiller history = new LibDBgate.HistoryFiller();
                LibMESone.Tables.Tapplication application = new LibMESone.Tables.Tapplication(); application.Put(LibMESone.Tables.Tapplication.EKeys.APPINFO, Lib.Global.AppInfo());

                Handlers.HandlerDatabase database = new Handlers.HandlerDatabase(config_file.DB_TYPE,
                                                               config_file.CONNECTION_STRING,
                                                               settings, clients, tags,
                                                               rt_values, history, application, diagnostics);

                LibMESone.Loggers.DBLogger db_logger = new LibMESone.Loggers.DBLogger(database.Database);

                Handlers.HandlerData data = new Handlers.HandlerData(buffer, rt_values, history, application);

                Connections connections = new Connections(clients, tags, buffer, diagnostics);

                LibDBgate.HistoryCleaner history_cleaner = new LibDBgate.HistoryCleaner(database.Database, settings.DEPTH_HISTORY_HOUR);
                LibMESone.Loggers.TextLogCleaner text_log_cleaner = new LibMESone.Loggers.TextLogCleaner(text_logger, config_file.DEPTH_LOG_DAY);
                LibMESone.Loggers.DBLogCleaner db_log_cleaner = new LibMESone.Loggers.DBLogCleaner(db_logger, settings.DEPTH_LOG_DAY);

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
