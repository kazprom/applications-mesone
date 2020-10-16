using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;

namespace LibDBgate
{
    public class BaseTableTags : BaseTableNDE
    {

        public const string col_name_clients_id = "clients_id";
        public const string col_name_rate = "rate";
        public const string col_name_data_type = "data_type";
        public const string col_name_rt_values_enabled = "rt_values_enabled";
        public const string col_name_history_enabled = "history_enabled";


        public BaseTableTags()
        {
            source.Columns.Add(col_name_clients_id, typeof(int)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
            {
                data_type = OdbcType.BigInt,
            });
            source.Columns.Add(col_name_rate, typeof(int)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
            {
                data_type = OdbcType.SmallInt,
            });
            source.Columns.Add(col_name_data_type, typeof(byte)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
            {
                data_type = OdbcType.TinyInt,
            });
            source.Columns.Add(col_name_rt_values_enabled, typeof(bool)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
            {
                data_type = OdbcType.TinyInt,
            });
            source.Columns.Add(col_name_history_enabled, typeof(bool)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
            {
                data_type = OdbcType.TinyInt,
            });

        }
    }
}
