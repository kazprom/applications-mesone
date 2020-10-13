using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;

namespace S7_DB_gate
{




    public class Tags
    {

        #region CONSTANTS

        public const string col_name_id = "id";
        public const string col_name_clients_id = "clients_id";
        public const string col_name_plc_data_type = "plc_data_type";
        public const string col_name_db_no = "db_no";
        public const string col_name_db_offset = "db_offset";
        public const string col_name_rate = "rate";
        public const string col_name_req_type = "req_type";
        public const string col_name_data_type = "data_type";
        public const string col_name_rt_values_enabled = "rt_values_enabled";
        public const string col_name_history_enabled = "history_enabled";

        #endregion


        #region PROPERTIES

        private DataTable source = new DataTable("tags");
        public DataTable Source { get { return source; } }

        #endregion


        public Tags()
        {
            
            source.Columns.Add(col_name_id, typeof(int)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp() { primary_key = true });
            source.Columns.Add(col_name_clients_id, typeof(int));
            source.Columns.Add(col_name_plc_data_type, typeof(string));
            source.Columns.Add(col_name_db_no, typeof(int));
            source.Columns.Add(col_name_db_offset, typeof(int));
            source.Columns.Add(col_name_rate, typeof(int));
            source.Columns.Add(col_name_req_type, typeof(string));
            source.Columns.Add(col_name_data_type, typeof(byte));
            source.Columns.Add(col_name_rt_values_enabled, typeof(bool));
            source.Columns.Add(col_name_history_enabled, typeof(bool));

        }
    }
}
