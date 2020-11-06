using System;
using System.Collections.Generic;
using System.Text;

namespace LibDBgate.Structs
{
    class Tt_ : LibMESone.Structs.BaseID
    {
        /*

        #region CONSTANTS

        public const char separator = '_';

        public const string table_prefix = "t";

        public const string col_name_tags_id = "tags_id";
        public Lib.Database.SExtProp prop_tags_id = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.BigInt
        };

        public const string col_name_timestamp = "timestamp";
        public Lib.Database.SExtProp prop_timestamp = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.Timestamp,
            size = 3
        };

        public const string col_name_value = "value";
        public Lib.Database.SExtProp prop_value = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.Binary,
            size = 8
        };

        public const string col_name_quality = "quality";
        public Lib.Database.SExtProp prop_quality = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.SmallInt
        };



        #endregion




        public Tt_(DateTime dt)
        {
            source.TableName = GetTableName(dt);

            source.Columns.Add(col_name_tags_id, typeof(int)).ExtendedProperties.Add(prop_tags_id.GetType(), prop_tags_id);
            source.Columns.Add(col_name_timestamp, typeof(DateTime)).ExtendedProperties.Add(prop_timestamp.GetType(),prop_timestamp);
            source.Columns.Add(col_name_value, typeof(byte[])).ExtendedProperties.Add(prop_value.GetType(), prop_value);
            source.Columns.Add(col_name_quality, typeof(byte)).ExtendedProperties.Add(prop_quality.GetType(),prop_quality); 

        }


        public static string GetTableName(DateTime timestamp)
        {
            return table_prefix + separator + timestamp.ToString("yyyy" + separator + "MM" + separator + "dd" + separator + "HH");
        }

        */
    }
}
