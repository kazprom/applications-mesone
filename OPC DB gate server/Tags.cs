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

        public const string col_name_clients_id = "clients_id";
        public const string col_name_path = "path";
        public const string col_name_rate = "rate";
        public const string col_name_data_type = "data_type";

        #endregion


        #region PROPERTIES

        private DBTable source = new DBTable("tags");
        public DBTable Source { get { return source; } }


        private Dictionary<int, List<OPC_DB_gate_Lib.TagSettings>> dictionary = new Dictionary<int, List<OPC_DB_gate_Lib.TagSettings>>();
        public Dictionary<int, List<OPC_DB_gate_Lib.TagSettings>> Dictionary { get { return dictionary; } }

        #endregion


        public Tags()
        {

            source.AddColumn(col_name_clients_id, typeof(int));
            source.AddColumn(col_name_path, typeof(string));
            source.AddColumn(col_name_rate, typeof(int));
            source.AddColumn(col_name_data_type, typeof(byte));
        }
    }
}
