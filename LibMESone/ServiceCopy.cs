using LibMESone.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace LibMESone
{
    public class ServiceCopy : IDisposable
    {

        #region VARIABLES

        protected bool disposedValue;

        protected string name;
        protected Lib.Parameter<bool> enabled;
        protected Lib.Parameter<string> db_type;
        protected Lib.Parameter<string> connection_string;

        #endregion

        /*
        public ServiceCopy(Tservices.Row r_services, Tdatabases.Row r_databases, Thosts.Row r_hosts)
        {
            try
            {
                //ID = id;
               // UpdateSettings(name, enabled, db_type, connection_string);
                Lib.Message.Make($"Service {name} created");
            }
            catch (Exception ex)
            {
                throw new Exception("Error constructor", ex);
            }
        }
        */

        public void UpdateSettings(string name, bool enabled, string db_type, string connection_string)
        {
            try
            {
               // NAME = name;
               // ENABLED = enabled;
               // DATABASE_ID = row.databases_id;
            }
            catch (Exception ex)
            {
                throw new Exception("Error update settings", ex);
            }
        }


        protected virtual void Dispose(bool disposing) { }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
            Lib.Message.Make($"Service {name} disposed");
        }
    }
}
