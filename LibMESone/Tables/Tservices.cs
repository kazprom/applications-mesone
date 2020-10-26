using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Text;

namespace LibMESone.Tables
{
    public class Tservices:TbaseNE
    {

        #region CONSTANTS

        public const string col_name_databases_id = "databases_id";
        public Lib.Database.SExtProp prop_databases_id = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.BigInt
        };

        public const string col_name_service_types_id = "service_types_id";
        public Lib.Database.SExtProp prop_service_types_id = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.BigInt
        };

        #endregion


        public Tservices()
        {
            source.TableName = "services";

            source.Columns.Add(col_name_databases_id, typeof(long)).ExtendedProperties.Add(prop_databases_id.GetType(), prop_databases_id);
            source.Columns.Add(col_name_service_types_id, typeof(long)).ExtendedProperties.Add(prop_service_types_id.GetType(), prop_service_types_id);

        }

    }
}
