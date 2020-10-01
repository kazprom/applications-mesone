using Lib;
using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;

[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("OPC DB Gate Server")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("1.0.*")]


namespace OPC_DB_gate_server
{
    class Program
    {

        #region CONSTANTS

        const string par_name_db_type = "DB_TYPE";
        const string par_name_connection_string = "CONNECTION_STRING";
        const string par_name_depth_log_day = "DEPTH_LOG_DAY";

        #endregion

        static Config config = new Config();
        static Database database = new Database();
        static DataSet read_tables = new DataSet();
        static DataSet write_tables = new DataSet();
        static TCPclients tcp_clients = new TCPclients();


        static void Main(string[] args)
        {
            Lib.Global.PrintAppInfo();

            if (args.Count() >= 1)
            {
                config.PathFile = args[0];
            }
            else
            {
                config.PathFile = $"{Lib.Global.NameExeFile.Split('.')[0]}.xml";
            }


            Global.Subscribe_Ctrl_C();

            FillConfig();
            FillTables();

            while (true)
            {
                try
                {

#if DEBUG
                    if (DateTime.Now.Second % 5 == 0)
#else
                    if(DateTime.Now.Second == 0)
#endif
                    {
                        config.Read();

                        database.Type = config.Get(par_name_db_type);
                        database.ConnectionString = config.Get(par_name_connection_string);
                        database.Read(ref read_tables);
                        


                        Thread.Sleep(1000);
                    }



                    Thread.Sleep(100);

                }
                catch (Exception ex)
                {
                    Logger.WriteMessage("Error execution", ex);
                    Thread.Sleep(5000);
                }
            }
        }

        static void FillConfig()
        {
            config.Add(par_name_db_type, database.Type);
            config.Add(par_name_connection_string, database.ConnectionString);
            config.Add(par_name_depth_log_day, "2");
        }

        static void FillTables()
        {
            DataTable dt;
            
            dt = new DataTable("settings");
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("key", typeof(string));
            dt.Columns.Add("value", typeof(string));
            read_tables.Tables.Add(dt);
            
            read_tables.Tables.Add(tcp_clients.settings);

            
            dt = new DataTable("tags");
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("clients_id", typeof(int));
            dt.Columns.Add("path", typeof(string));
            dt.Columns.Add("rate", typeof(short));
            dt.Columns.Add("data_type", typeof(byte));
            read_tables.Tables.Add(dt);
            
        }

    }
}
