using Lib;
using System.Data;

namespace OPC_DB_gate_server
{
    static class Settings
    {

        public static class XML
        {
            public static Lib.XML file = new Lib.XML();

            static XML()
            {
                file.Path = $"{Global.NameExeFile.Split('.')[0]}.xml";
            }

        }

        public static class Database
        {
            public static DBTable table = new DBTable("settings");

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
