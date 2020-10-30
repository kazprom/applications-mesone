using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;

namespace LibMESone.Tables
{
    public class Services : BaseNE
    {
        [Field(Field.Etype.BigInt)]
        public long Databases_id { get; set; }

        [Field(Field.Etype.BigInt)]
        public long Service_types_id { get; set; }

        public Services()
        {
            container.TableName = "services";
        }


        /*
        public new class Row : TbaseNE.Row
        {
            public long databases_id;
            public long service_types_id;

            public static void DataRowToRow(DataRow row, ref Row r_row)
            {
                TbaseNE.Row b_row = r_row;
                TbaseNE.Row.DataRowToRow(row, ref b_row);
                r_row.databases_id = (long)row[col_name_databases_id];
                r_row.service_types_id = (long)row[col_name_service_types_id];
            }
        }


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
        */
    }
}
