using Lib;

namespace OPC_DB_gate_server
{
    static class Application
    {
        public static class Database
        {

            public static DBTable table = new DBTable("application");

            public enum EColumns
            {
                key,
                value
            }

            static Database()
            {
                table.AddColumn(EColumns.key.ToString(), typeof(string));
                table.AddColumn(EColumns.value.ToString(), typeof(string));
            }

        }
    }
}
