using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LibMESone
{
    public class DBcore
    {

        #region VARIABLES

        private Lib.Parameter<Lib.Database.EType> db_type;
        private Lib.Parameter<string> connection_string;
        private Timer timer;

        #endregion


        #region PROPERTIES

        private Tables.Thosts hosts = new Tables.Thosts();
        public Tables.Thosts Hosts { get { return hosts; } }

        private Tables.Tdatabases databases = new Tables.Tdatabases();
        public Tables.Tdatabases Databases { get { return databases; } }

        private Tables.Tservice_types service_types = new Tables.Tservice_types();
        public Tables.Tservice_types ServiceTypes { get { return service_types; } }

        private Tables.Tservices services = new Tables.Tservices();
        public Tables.Tservices Services { get { return services; } }


        private Lib.Database database = new Lib.Database();
        public Lib.Database Database { get { return database; } }

        #endregion


        #region EVENTS



        #endregion

        public DBcore(Lib.Parameter<Lib.Database.EType> db_type, Lib.Parameter<string> connection_string)
        {

            this.db_type = db_type;
            Type_ValueChanged(this.db_type.Value);
            this.db_type.ValueChanged += Type_ValueChanged;

            this.connection_string = connection_string;
            Connection_string_ValueChanged(this.connection_string.Value);
            this.connection_string.ValueChanged += Connection_string_ValueChanged;

            timer = new Timer(ReadAction, null, 0, 60000);

        }

        private void ReadAction(object state)
        {
            try
            {

                if (database != null)
                {
                    if (service_types != null)
                        if (!database.Read(service_types.Source))
                            Lib.Message.Make("Can't read service types");

                    
                    if (services != null)
                        if (!database.Read(services.Source))
                            Lib.Message.Make("Can't read services");

                    
                    if (databases != null)
                        if (!database.Read(databases.Source))
                            Lib.Message.Make("Can't read databases");

                    if (hosts != null)
                        if (!database.Read(hosts.Source))
                            Lib.Message.Make("Can't read hosts");

                }
            }
            catch (Exception ex)
            {
                Lib.Message.Make("Error read", ex);
            }



        }

        private void Connection_string_ValueChanged(string value)
        {
            database.ConnectionString = value;
        }

        private void Type_ValueChanged(Lib.Database.EType value)
        {
            database.Type = value;
        }

    }
}
