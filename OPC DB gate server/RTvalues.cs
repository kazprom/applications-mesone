using Lib;
using System;

namespace OPC_DB_gate_server
{
    static class RTvalues
    {

        public static class Database
        {

            public static DBTable table = new DBTable("rt_values");

            public enum EColumns
            {
                tags_id,
                timestamp,
                value_raw,
                value_str,
                quality
            }

            static Database()
            {
                table.AddColumn(EColumns.tags_id.ToString(), typeof(int)); ;
                table.AddColumn(EColumns.timestamp.ToString(), typeof(DateTime));
                table.AddColumn(EColumns.value_raw.ToString(), typeof(byte[]));
                table.AddColumn(EColumns.value_str.ToString(), typeof(string));
                table.AddColumn(EColumns.quality.ToString(), typeof(byte));
            }

        }
    }
}
