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

        private Timer timer;

        #endregion

        #region PROPERTIES

        public string Name { get { return "CORE"; } }

        #endregion

        #region CONTRUCTOR

        public Core(Type service_type)
        {
            try
            {

                this.service_type = service_type;

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
                };

                timer = new Timer(GetServices, null, 0, period);

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

        private void GetServices(object state)
        {
            try
            {
                if (database != null)
                {

                    IEnumerable<Structs.ServiceTypes> service_types = database.Read<Structs.ServiceTypes>("service_types");
                    Structs.ServiceTypes service_type = null;

                    if (service_types == null)
                        logger.Warn("Can't read service types");
                    else
                        service_type = service_types.First(x => x.Guid.Equals(Lib.Common.AppGUID(), StringComparison.OrdinalIgnoreCase));


                    IEnumerable<Structs.Services> services = database.Read<Structs.Services>("services");
                    if (services == null)
                        logger.Warn("Can't read services");

                    IEnumerable<Structs.Databases> databases = database.Read<Structs.Databases>("databases");
                    if (databases == null)
                        logger.Warn("Can't read databases");

                    IEnumerable<Structs.Hosts> hosts = database.Read<Structs.Hosts>("hosts");
                    if (hosts == null)
                        logger.Warn("Can't read hosts");

                    if (service_type == null)
                        logger.Warn("Can't find id of service");
                    else if (services != null && database != null && hosts != null)
                    {

                        IEnumerable<Structs.Services> enabled_services = services.Where(x => x.Service_types_id == service_type.Id).Where(x => (bool)x.Enabled);
                        IEnumerable<Structs.Databases> enabled_databases = databases.Where(x => (bool)x.Enabled);
                        IEnumerable<Structs.Hosts> enabled_hosts = hosts.Where(x => (bool)x.Enabled);

                        IEnumerable<DatabasesHosts> dh;

                        dh = enabled_databases
                            .Join(
                            enabled_hosts,
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

                        sdh = enabled_services
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
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Name}. Get services");
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
