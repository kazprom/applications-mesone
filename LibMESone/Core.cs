﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace LibMESone
{
    public class Core
    {

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

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

                        IEnumerable<Structs.Services> enabled_services = services.Where(x => x.Service_types_id == service_type.Id).Where(x => x.Enabled);
                        IEnumerable<Structs.Databases> enabled_databases = databases.Where(x => x.Enabled);
                        IEnumerable<Structs.Hosts> enabled_hosts = hosts.Where(x => x.Enabled);

                        IEnumerable<Structs.DatabasesHosts> dh;

                        dh = enabled_databases
                            .Join(
                            enabled_hosts,
                            db => db.Hosts_id,
                            host => host.Id,
                            (db, host) => new Structs.DatabasesHosts
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

                        IEnumerable<Structs.ServicesDatabasesHosts> sdh;

                        sdh = enabled_services
                            .Join(
                            dh,
                            srv => srv.Databases_id,
                            d_h => d_h.Id,
                            (srv, d_h) => new Structs.ServicesDatabasesHosts
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
                            Structs.ServicesDatabasesHosts set_service = sdh.First(x => x.Id == service_id);

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
                            Structs.ServicesDatabasesHosts set_service = sdh.First(x => x.Id == service_id);
                            Service inst_service = (Service)Activator.CreateInstance(this.service_type, this, set_service.Name);

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
                logger.Error(ex);
            }
        }

        #endregion

    }
}
