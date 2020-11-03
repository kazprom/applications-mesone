using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using LibMESone.Tables;

namespace LibMESone
{
    public class Core
    {

        #region VARIABLES

        private ConfigFile config_file;
        private Type service_type;
        private Dictionary<long, Service> services = new Dictionary<long, Service>();

        private Lib.Database database;
        private Timer timer;

        #endregion


        #region CONTRUCTOR

        public Core(ConfigFile config_file, Type service_type)
        {

            this.config_file = config_file;
            this.service_type = service_type;

            database = new Lib.Database(this.config_file.DB_DRIVER,
                                        this.config_file.DB_HOST,
                                        this.config_file.DB_PORT,
                                        this.config_file.DB_CHARSET,
                                        this.config_file.DB_BASE_NAME,
                                        this.config_file.DB_USER,
                                        this.config_file.DB_PASSWORD);

            timer = new Timer(GetServices, null, 0, 60000);

        }
        #endregion


        #region PRIVATES

        private void GetServices(object state)
        {


            try
            {

                if (database != null)
                {

                    IEnumerable<ServiceTypes> service_types = database.Read<ServiceTypes>("service_types");
                    ServiceTypes service_type = null;

                    if (service_types == null)
                        Lib.Message.Make("Can't read service types");
                    else
                        service_type = service_types.First(x => x.Guid.Equals(Lib.Common.AppGUID(), StringComparison.OrdinalIgnoreCase));


                    IEnumerable<Services> services = database.Read<Services>("services");
                    if (services == null)
                        Lib.Message.Make("Can't read services");

                    IEnumerable<Databases> databases = database.Read<Databases>("databases");
                    if (databases == null)
                        Lib.Message.Make("Can't read databases");

                    IEnumerable<Hosts> hosts = database.Read<Hosts>("hosts");
                    if (hosts == null)
                        Lib.Message.Make("Can't read hosts");

                    if (service_type == null)
                        Lib.Message.Make("Can't find id of service");
                    else if (services != null && database != null && hosts != null)
                    {

                        IEnumerable<Services> enabled_services = services.Where(x => x.Service_types_id == service_type.Id).Where(x => x.Enabled);
                        IEnumerable<Databases> enabled_databases = databases.Where(x => x.Enabled);
                        IEnumerable<Hosts> enabled_hosts = hosts.Where(x => x.Enabled);

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

                        IEnumerable<long> fresh_ids = sdh.Select(x => x.Id);
                        IEnumerable<long> existing_ids = this.services.Keys;

                        IEnumerable<long> waste = existing_ids.Except(fresh_ids);
                        IEnumerable<long> modify = fresh_ids.Intersect(existing_ids);
                        IEnumerable<long> missing = fresh_ids.Except(existing_ids);

                        foreach (long service_id in waste)
                        {
                            this.services[service_id].Dispose();
                            this.services.Remove(service_id);
                        }

                        foreach (long service_id in modify)
                        {
                            ServicesDatabasesHosts set_service = sdh.First(x => x.Id == service_id);

                            this.services[service_id].UpdateSettings(set_service.Driver,
                                                                     set_service.Host,
                                                                     set_service.Port,
                                                                     set_service.Charset,
                                                                     set_service.Database,
                                                                     set_service.Username,
                                                                     set_service.Password);
                        }

                        foreach (long service_id in missing)
                        {
                            ServicesDatabasesHosts set_service = sdh.First(x => x.Id == service_id);
                            Service inst_service = (Service)Activator.CreateInstance(this.service_type, set_service.Name);

                            inst_service.UpdateSettings(set_service.Driver,
                                                        set_service.Host,
                                                        set_service.Port,
                                                        set_service.Charset,
                                                        set_service.Database,
                                                        set_service.Username,
                                                        set_service.Password);

                            this.services.Add(set_service.Id, inst_service);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Lib.Message.Make("Error read", ex);
            }
        }

        #endregion

    }
}
