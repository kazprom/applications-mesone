using System;
using System.Collections.Generic;
using System.Text;

namespace LibMESone
{
    class DBcore
    {


        #region PROPERTIES

        private Thosts hosts = new Thosts();
        public Thosts Hosts { get { return hosts; } }

        private Tdatabases databases = new Tdatabases();
        public Tdatabases Databases { get { return databases; } }

        private Tservice_types service_types = new Tservice_types();
        public Tservice_types ServiceTypes { get { return service_types; } }

        private Tservices services = new Tservices();
        public Tservices Services { get { return services; } }

        #endregion


        public DBcore()
        {




        }


    }
}
