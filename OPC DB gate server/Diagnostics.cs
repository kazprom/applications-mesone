using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPC_DB_gate_server
{
    static class Diagnostics
    {

        public static class Database
        {
            public static DBTable table = new DBTable("diagnostics");

            public enum EColumns
            {
                clients_id,
                timestamp,
                message
            }

            static Database()
            {
                table.AddColumn("clients_id", typeof(int));
                table.AddColumn("timestamp", typeof(DateTime));
                table.AddColumn("message", typeof(string));
            }
        }




    }
}
