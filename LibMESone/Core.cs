using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Reflection;

namespace LibMESone
{
    public class Core : IDisposable
    {

        #region CONSTANTS

#if DEBUG
        private const int period = 5000;
#else
        private const int period = 60000;
#endif

        #endregion

        #region VARIABLES

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private Type service_type;

        private ConfigFile config_file = new ConfigFile();
        private Dictionary<ulong, Service> services = new Dictionary<ulong, Service>();

        private Lib.Database database;

        private Timer timer_DB;

        #endregion

        #region PROPERTIES

        public string Name { get { return "CORE"; } }

        public Structs.ServiceDiagnostic ServiceDiagnostic { get; private set; }

        #endregion

        #region CONTRUCTOR

        public Core(Type service_type)
        {
            try
            {

                this.service_type = service_type;

                ServiceDiagnostic = new Structs.ServiceDiagnostic() { Version = Lib.Common.AppVersion() };

                config_file.ReadCompleted += (ConfigFile sender) =>
                {
                    if (database == null) database = new Lib.Database(0);

                    database.LoadSettings(Name,
                                          sender.DB_Driver,
                                          sender.DB_Host,
                                          sender.DB_Port,
                                          sender.DB_Charset,
                                          sender.DB_BaseName,
                                          sender.DB_User,
                                          sender.DB_Password);

                    NLog.Targets.FileTarget target = (NLog.Targets.FileTarget)NLog.LogManager.Configuration.FindTargetByName("file");
                    target.MaxArchiveFiles = (int)sender.LOG_DepthDay;
                };

                timer_DB = new Timer(DB_Handler, null, 0, period);

            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Name}. Constructor");
            }
        }

        #endregion

        #region DESTRUCTOR

        ~Core()
        {
            logger.Info($"{Name}. Disposed");
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (Service service in services.Values)
                    {
                        service.Dispose();
                    }

                    services.Clear();

                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region PRIVATES

        private void DB_Handler(object state)
        {
            try
            {
                if (database != null)
                {

                    //----------read---------------
                    IEnumerable<Structs.ServiceType> service_types = null;
                    if (database.CompareTableSchema<Structs.ServiceType>("service_types"))
                        service_types = database.WhereRead<Structs.ServiceType>("service_types", new { Guid = Lib.Common.AppGUID() });
                    else
                    {
                        logger.Warn("Can't read service types");
                        return;
                    }

                    if (service_types == null || service_types.Count() == 0)
                    {
                        logger.Warn("Can't find id of service");
                        return;
                    }


                    IEnumerable<Structs.Service> services = null;
                    if (database.CompareTableSchema<Structs.Service>("services"))
                    {

                        ServiceDiagnostic.Service_types_id = service_types.First().Id;

                        services = database.WhereRead<Structs.Service>("services", new
                        {
                            Service_types_id = ServiceDiagnostic.Service_types_id,
                            Enabled = true
                        });
                    }

                    else
                    {
                        logger.Warn("Can't read services");
                        return;
                    }


                    IEnumerable<Structs.Databases> databases = null;
                    if (database.CompareTableSchema<Structs.Databases>("databases"))
                        databases = database.WhereRead<Structs.Databases>("databases", new { Enabled = true });
                    else
                    {
                        logger.Warn("Can't read databases");
                        return;
                    }

                    IEnumerable<Structs.Host> hosts = null;
                    if (database.CompareTableSchema<Structs.Host>("hosts"))
                        hosts = database.WhereRead<Structs.Host>("hosts", new { Enabled = true });
                    else
                    {
                        logger.Warn("Can't read hosts");
                        return;
                    }

                    if (services != null && databases != null && hosts != null)
                    {

                        IEnumerable<DatabasesHosts> dh;

                        dh = databases
                            .Join(
                            hosts,
                            db => db.Hosts_id,
                            host => host.Id,
                            (db, host) => new DatabasesHosts
                            {
                                Id = db.Id,
                                Database = db.Database,
                                Driver = db.Driver,
                                Port = db.Port,
                                Charset = db.Charset,
                                Username = db.Username,
                                Password = db.Password,
                                Host = host.Ip
                            }
                            );

                        IEnumerable<ServicesDatabasesHosts> sdh;

                        sdh = services
                            .Join(
                            dh,
                            srv => srv.Databases_id,
                            d_h => d_h.Id,
                            (srv, d_h) => new ServicesDatabasesHosts
                            {
                                Id = srv.Id,
                                Name = srv.Name,
                                Database = d_h.Database,
                                Driver = d_h.Driver,
                                Host = d_h.Host,
                                Port = d_h.Port,
                                Charset = d_h.Charset,
                                Username = d_h.Username,
                                Password = d_h.Password
                            }
                            );

                        IEnumerable<ulong> fresh_ids = sdh.Select(x => (ulong)x.Id);
                        IEnumerable<ulong> existing_ids = this.services.Keys;

                        IEnumerable<ulong> waste = existing_ids.Except(fresh_ids);
                        IEnumerable<ulong> modify = fresh_ids.Intersect(existing_ids);
                        IEnumerable<ulong> missing = fresh_ids.Except(existing_ids);

                        foreach (ulong service_id in waste)
                        {
                            this.services[service_id].Dispose();
                            this.services.Remove(service_id);
                        }

                        foreach (ulong service_id in modify)
                        {
                            ServicesDatabasesHosts set_service = sdh.First(x => x.Id == service_id);

                            this.services[service_id].LoadDatabaseSettings((ulong)set_service.Id,
                                                                           set_service.Name,
                                                                           set_service.Driver,
                                                                           set_service.Host,
                                                                           set_service.Port,
                                                                           set_service.Charset,
                                                                           set_service.Database,
                                                                           set_service.Username,
                                                                           set_service.Password);
                        }

                        foreach (ulong service_id in missing)
                        {
                            ServicesDatabasesHosts set_service = sdh.First(x => x.Id == service_id);
                            Service inst_service = (Service)Activator.CreateInstance(this.service_type, this, set_service.Id);

                            inst_service.LoadDatabaseSettings((ulong)set_service.Id,
                                                              set_service.Name,
                                                              set_service.Driver,
                                                              set_service.Host,
                                                              set_service.Port,
                                                              set_service.Charset,
                                                              set_service.Database,
                                                              set_service.Username,
                                                              set_service.Password);

                            this.services.Add((ulong)set_service.Id, inst_service);
                        }
                    }

                    //----------write---------------

                    ServiceDiagnostic.Sys_ts = DateTime.Now;
                    database.Update(Structs.ServiceDiagnostic.TableName, ServiceDiagnostic);

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Name}. DB Handler");
            }
        }

        private class ServicesDatabasesHosts : DatabasesHosts
        {

            public string Name { get; set; }


        }

        private class DatabasesHosts : Structs.BaseID
        {

            public string Database { get; set; }

            public string Driver { get; set; }

            public string Host { get; set; }

            public uint Port { get; set; }

            public string Charset { get; set; }

            public string Username { get; set; }

            public string Password { get; set; }


        }


        #endregion

    }
}
