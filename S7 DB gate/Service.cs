using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace S7_DB_gate
{
    class Service : LibMESone.ServiceCopy
    {
        /*

        #region CONSTRUCTOR

        public Service(LibMESone.Tables.Tservices.Row row):base (null, null, null)
        {


            Lib.Buffer<LibDBgate.TagData> buffer = new Lib.Buffer<LibDBgate.TagData>(10000);

            Tables.Tsettings settings = new Tables.Tsettings();
            Tables.Tclients clients = new Tables.Tclients();
            Tables.Ttags tags = new Tables.Ttags();
            Tables.Tdiagnostics diagnostics = new Tables.Tdiagnostics();

            LibDBgate.Trt_values rt_values = new LibDBgate.Trt_values();
            LibDBgate.HistoryFiller history = new LibDBgate.HistoryFiller();
            LibMESone.Tables.Tapplication application = new LibMESone.Tables.Tapplication(); application.Put(LibMESone.Tables.Tapplication.EKeys.APPINFO, Lib.Global.AppInfo());
            
            /*
            Handlers.HandlerDatabase database = new Handlers.HandlerDatabase(this.db_type, this.connection_string,
                                                           settings, clients, tags,
                                                           rt_values, history, application, diagnostics);
            */
            //LibMESone.Loggers.DBLogger db_logger = new LibMESone.Loggers.DBLogger(database.Database);

         //   Handlers.HandlerData data = new Handlers.HandlerData(buffer, rt_values, history, application);

            //  Connections connections = new Connections(clients, tags, buffer, diagnostics);
            /*
            LibDBgate.HistoryCleaner history_cleaner = new LibDBgate.HistoryCleaner(database.Database, settings.DEPTH_HISTORY_HOUR);
            */
            //LibMESone.Loggers.DBLogCleaner db_log_cleaner = new LibMESone.Loggers.DBLogCleaner(db_logger, settings.DEPTH_LOG_DAY);

       // }
    /*
        #endregion





        private new void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                   

                }
                disposedValue = true;
            }
        }

        */
    }
}
