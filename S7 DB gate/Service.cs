using System;
using System.Collections.Generic;
using System.Linq;

namespace S7_DB_gate
{
    class Service : LibMESone.Service
    {
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private Dictionary<long, S7Client> points = new Dictionary<long, S7Client>();


        public Service(string name) : base(name)
        {
        }


        public override void Handler(object state)
        {
            try
            {
                if (database != null)
                {

                    IEnumerable<LibDBgate.Tables.Setting> settings = database.Read<LibDBgate.Tables.Setting>("settings");
                    IEnumerable<Structs.Client> clients = database.Read<Structs.Client>("clients");
                    IEnumerable<Structs.Tag> tags = database.Read<Structs.Tag>("tags");

                    if (clients != null && tags != null)
                    {
                        IEnumerable<Structs.Client> enabled_clients = clients.Where(x => x.Enabled);


                        IEnumerable<long> fresh_ids = enabled_clients.Select(x => x.Id);
                        IEnumerable<long> existing_ids = this.points.Keys;

                        IEnumerable<long> waste = existing_ids.Except(fresh_ids);
                        IEnumerable<long> modify = fresh_ids.Intersect(existing_ids);
                        IEnumerable<long> missing = fresh_ids.Except(existing_ids);

                        foreach (long point_id in waste)
                        {
                            points[point_id].Dispose();
                            points.Remove(point_id);
                        }

                        foreach (long point_id in modify)
                        {
                            Structs.Client set_point = enabled_clients.First(x => x.Id == point_id);

                            points[point_id].UpdateSettings(set_point.Cpu_type,
                                                            set_point.Ip,
                                                            set_point.Port,
                                                            set_point.Rack,
                                                            set_point.Slot);
                        }

                        foreach (long point_id in missing)
                        {
                            Structs.Client set_point = enabled_clients.First(x => x.Id == point_id);
                            S7Client inst_point = new S7Client(set_point.Name);

                            inst_point.UpdateSettings(set_point.Cpu_type,
                                                      set_point.Ip,
                                                      set_point.Port,
                                                      set_point.Rack,
                                                      set_point.Slot);

                            points.Add(set_point.Id, inst_point);
                        }


                        IEnumerable<Structs.Tag> enabled_tags = tags.Where(x => x.Enabled);
                        IEnumerable<long> client_ids = enabled_tags.GroupBy(x => x.Clients_id).Select(x => x.First()).Select(x => x.Clients_id);

                        foreach (long id in client_ids)
                        {
                            if(points.ContainsKey(id))
                            {
                                points[id].LoadTags(enabled_tags.Where(x => x.Clients_id == id));
                            }
                        }

                    }
                }
            }
            catch (Exception)
            {

            }

        }


        public override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {


                }
                disposedValue = true;
            }
        }

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





            

            */
    }
}
