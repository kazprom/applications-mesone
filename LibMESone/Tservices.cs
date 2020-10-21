using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Text;

namespace LibMESone
{
    class Tservices:TbaseNE
    {

        #region CONSTANTS

        public const string col_name_databases_id = "databases_id";
        public readonly static Type data_type_databases_id = typeof(long);

        public const string col_name_service_types_id = "service_types_id";
        public readonly static Type data_type_service_types_id = typeof(long);

        #endregion


        public Tservices()
        {
            source.TableName = "services";

            source.Columns.Add(col_name_databases_id, data_type_databases_id).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
            {
                data_type = OdbcType.BigInt
            });

            source.Columns.Add(col_name_service_types_id, data_type_service_types_id).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
            {
                data_type = OdbcType.BigInt
            });


        }

    }
}
