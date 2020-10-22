using System;
using System.Collections.Generic;
using System.Text;

namespace LibMESone
{
    class DBcore
    {


        #region PROPERTIES

        private Tables.Thosts hosts = new Tables.Thosts();
        public Tables.Thosts Hosts { get { return hosts; } }

        private Tables.Tdatabases databases = new Tables.Tdatabases();
        public Tables.Tdatabases Databases { get { return databases; } }

        private Tables.Tservice_types service_types = new Tables.Tservice_types();
        public Tables.Tservice_types ServiceTypes { get { return service_types; } }

        private Tables.Tservices services = new Tables.Tservices();
        public Tables.Tservices Services { get { return services; } }

        #endregion


        public DBcore()
        {




        }


    }
}
