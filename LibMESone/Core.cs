using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Linq;
using LibMESone.Tables;
using Lib;
using SqlKata;

namespace LibMESone
{
    public class Core
    {

        #region VARIABLES

        private ConfigFile config_file;
        //private Lib.Parameter<string> service_guid;

        private Lib.Database database;



        // private Lib.Parameter<Lib.Database.EType> db_type;
        //private Lib.Parameter<string> connection_string;
        private Timer timer;

        #endregion


        #region PROPERTIES
        /*
        private Tables.Hosts hosts = new Tables.Hosts();
        public Tables.Hosts Hosts { get { return hosts; } }

        private Tables.Databases databases = new Tables.Databases();
        public Tables.Databases Databases { get { return databases; } }

        private Tables.ServiceTypes service_types = new Tables.ServiceTypes();
        public Tables.ServiceTypes ServiceTypes { get { return service_types; } }

        private Tables.Services services = new Tables.Services();
        public Tables.Services Services { get { return services; } }
        */

        // private Lib.Database database = new Lib.Database();
        // public Lib.Database Database { get { return database; } }

        #endregion


        #region EVENTS

        public delegate void GetedServicesNotify(IEnumerable<DatabasesHosts> result);  // delegate
        public event GetedServicesNotify GetedServices; // event


        #endregion


        #region CONTRUCTOR

        public Core(ConfigFile config_file)
        {

            this.config_file = config_file;

            database = new Lib.Database(config_file.DB_DRIVER,
                                        config_file.DB_HOST,
                                        config_file.DB_PORT,
                                        config_file.DB_CHARSET,
                                        config_file.DB_BASE_NAME,
                                        config_file.DB_USER,
                                        config_file.DB_PASSWORD);


            // this.db_type = db_type;
            // Type_ValueChanged(this.db_type.Value);
            // this.db_type.ValueChanged += Type_ValueChanged;

            //this.connection_string = connection_string;
            //Connection_string_ValueChanged(this.connection_string.Value);
            // this.connection_string.ValueChanged += Connection_string_ValueChanged;

            timer = new Timer(GetServices, null, 0, 60000);

        }
        #endregion


        #region PUBLICS

        /*

        public DataRow[] GetServicesByGUID(string GUID)
        {

            try
            {
                if (services != null && service_types != null)
                {

                    IEnumerable<DataRow> result = from DataRow types in service_types.Source.Rows
                                                  join DataRow copies in services.Source.Rows
                                                  on (long)types[Tservice_types.col_name_id] equals (long)copies[Tservices.col_name_service_types_id]
                                                  where (string)types[Tservice_types.col_name_guid] == GUID
                                                  select copies;

                    return result.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error get services data by GUID", ex);
            }

        }
        public Tservices.Row GetServiceDataByID(long id)
        {
            try
            {
                if (services != null)
                {
                    Tservices.Row result = new Tservices.Row();

                    DataRow dr = (from DataRow item in services.Source.Rows
                                  where (long)item[Tservices.col_name_id] == id
                                  select item).FirstOrDefault();

                    if (dr != null)
                    {
                        Tservices.Row.DataRowToRow(dr, ref result);
                        return result;
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error get database data by id", ex);
            }

            return null;
        }

        public Tdatabases.Row GetDatabaseDataByID(long id)
        {
            try
            {
                if (databases != null)
                {
                    Tdatabases.Row result = new Tdatabases.Row();

                    DataRow dr = (from DataRow item in databases.Source.Rows
                                  where (long)item[Tdatabases.col_name_id] == id
                                  select item).FirstOrDefault();

                    if (dr != null)
                    {
                        Tdatabases.Row.DataRowToRow(dr, ref result);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error get database data by id", ex);
            }
            return null;
        }

        public Thosts.Row GetHostDataByID(long id)
        {
            try
            {
                if (hosts != null)
                {
                    Thosts.Row result = new Thosts.Row();

                    DataRow dr = (from DataRow item in hosts.Source.Rows
                                  where (long)item[Thosts.col_name_id] == id
                                  select item).FirstOrDefault();

                    if (dr != null)
                    {
                        Thosts.Row.DataRowToRow(dr, ref result);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error get hosts data by id", ex);
            }
            return null;
        }
        */
        #endregion

        #region PRIVATES

        private void GetServices(object state)
        {


            try
            {

                if (database != null)
                {

                    IEnumerable<ServiceTypes> service_types = database.Read<ServiceTypes>("service_types");
                    if (service_types == null)
                    {
                        Lib.Message.Make("Can't read service types");
                    }
                    else
                    {
                        ServiceTypes service_type = service_types.Where(x => x.Guid.Equals(Lib.Global.AppGUID(), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                        if (service_type == null)
                        {
                            Lib.Message.Make("Can't find id of service");
                        }
                        else
                        {
                            IEnumerable<Services> services = database.Read<Services>("services");

                            if (services == null)
                            {
                                Lib.Message.Make("Can't read services");
                            }
                            else
                            {
                                IEnumerable<Services> enabled_services = services.Where(x => x.Enabled);

                                if (enabled_services.Count() > 0)
                                {
                                    IEnumerable<Databases> databases = database.Read<Databases>("databases");

                                    if (databases == null)
                                    {
                                        Lib.Message.Make("Can't read databases");
                                    }
                                    else
                                    {
                                        IEnumerable<Databases> enabled_databases = databases.Where(x => x.Enabled);

                                        if (enabled_databases.Count() > 0)
                                        {
                                            IEnumerable<Hosts> hosts = database.Read<Hosts>("hosts");

                                            if (hosts == null)
                                            {
                                                Lib.Message.Make("Can't read hosts");
                                            }
                                            else
                                            {
                                                IEnumerable<Hosts> enabled_hosts = hosts.Where(x => x.Enabled);

                                                if (enabled_hosts.Count() > 0)
                                                {
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

                                                    IEnumerable<DatabasesHosts> sdh;

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


                                                    GetedServices?.Invoke(sdh);
                                                }

                                            }
                                        }

                                    }


                                }


                                foreach (var item in services.Where(x => x.Service_types_id == service_type.Id))
                                {
                                    System.Console.WriteLine(item.Id);
                                }
                            }


                        }

                        /*
                        if (database.Read<Services>(services) == null)
                        {
                           
                        }
                        else
                        {
                            if (database.Read<Databases>(databases) == null)
                            {
                                Lib.Message.Make("Can't read databases");
                            }
                            else
                            {
                                if (database.Read<Hosts>(hosts) == null)
                                {
                                    Lib.Message.Make("Can't read hosts");
                                }
                                else
                                {
                                    DataTable result = null;


                                    GetedServices?.Invoke(null);
                                }

                            }

                        }
                        */
                    }





                }
            }
            catch (Exception ex)
            {
                Lib.Message.Make("Error read", ex);
            }
        }

        /*
        private void Connection_string_ValueChanged(string value)
        {
            database.ConnectionString = value;
        }

        private void Type_ValueChanged(Lib.Database.EType value)
        {
            database.Type = value;
        }

        */
        #endregion




    }
}
