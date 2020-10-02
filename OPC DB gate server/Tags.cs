using Lib;
using System.Data;

namespace OPC_DB_gate_server
{
    static class Tags
    {

        public static class Database
        {

            public static DBTable table = new DBTable("tags");

            public enum EColumns
            {
                clients_id,
                path,
                rate,
                data_type
            }

            static Database()
            {
                table.AddColumn(EColumns.clients_id.ToString(), typeof(int));
                table.AddColumn(EColumns.path.ToString(), typeof(string));
                table.AddColumn(EColumns.rate.ToString(), typeof(short));
                table.AddColumn(EColumns.data_type.ToString(), typeof(byte));
            }

        }




    }
}
