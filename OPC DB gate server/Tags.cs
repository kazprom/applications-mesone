using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;

namespace OPC_DB_gate_server
{




    public class Tags
    {

        #region CONSTANTS

        public const string col_name_id = "id";
        public const string col_name_clients_id = "clients_id";
        public const string col_name_path = "path";
        public const string col_name_rate = "rate";
        public const string col_name_data_type = "data_type";

        #endregion


        #region PROPERTIES

        private DataTable source = new DataTable("tags");
        public DataTable Source { get { return source; } }


        private Dictionary<int, List<LibOPCDBgate.TagSettings>> dictionary = new Dictionary<int, List<LibOPCDBgate.TagSettings>>();
        public Dictionary<int, List<LibOPCDBgate.TagSettings>> Dictionary { get { return dictionary; } }

        #endregion


        public Tags()
        {
            
            source.Columns.Add(col_name_id, typeof(int)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp() { primary_key = true });
            source.Columns.Add(col_name_clients_id, typeof(int));
            source.Columns.Add(col_name_path, typeof(string));
            source.Columns.Add(col_name_rate, typeof(int));
            source.Columns.Add(col_name_data_type, typeof(byte));

        }
    }
}
