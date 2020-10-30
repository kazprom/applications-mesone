using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Linq;
using LibMESone.Tables;

namespace LibMESone
{
    public class Core
    {

        #region VARIABLES

        private ConfigFile config_file;
        private Lib.Database database;



        // private Lib.Parameter<Lib.Database.EType> db_type;
        private Lib.Parameter<string> connection_string;
        private Timer timer;

        #endregion


        #region PROPERTIES

        private Tables.Hosts hosts = new Tables.Hosts();
        public Tables.Hosts Hosts { get { return hosts; } }

        private Tables.Databases databases = new Tables.Databases();
        public Tables.Databases Databases { get { return databases; } }

        private Tables.ServiceTypes service_types = new Tables.ServiceTypes();
        public Tables.ServiceTypes ServiceTypes { get { return service_types; } }

        private Tables.Services services = new Tables.Services();
        public Tables.Services Services { get { return services; } }


        // private Lib.Database database = new Lib.Database();
        // public Lib.Database Database { get { return database; } }

        #endregion


        #region EVENTS

        public delegate void ReadCompletedNotify();  // delegate
        public event ReadCompletedNotify ReadCompleted; // event


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

            timer = new Timer(ReadAction, null, 0, 60000);

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

        private void ReadAction(object state)
        {

            try
            {

                if (database != null)
                {
                    if (database.Read<ServiceTypes>(service_types) != null)
                        Lib.Message.Make("Can't read service types");

                    if (database.Read<Services>(services) != null)
                        Lib.Message.Make("Can't read services");

                    if (database.Read<Databases>(databases) != null)
                        Lib.Message.Make("Can't read databases");

                    if (database.Read<Hosts>(hosts) != null)
                        Lib.Message.Make("Can't read hosts");

                    ReadCompleted?.Invoke();

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
