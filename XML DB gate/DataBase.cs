using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace XML_DB_gate
{
    class DataBase
    {
        private Config config = Config.GetInstance();
        private Logger logger = Logger.GetInstance();

        #region VARIABLE
        private Exception test_connection_ex;
        private DbConnection connection = new OdbcConnection();
        private OdbcCommand command = new OdbcCommand();
        #endregion

        #region CONST

        const string varname_depth_log_day = "DEPTH_LOG_DAY";
        const string varname_key = "key";
        const string varname_value = "value";

        #endregion

        #region PROPERTIES

        private int depth_log_day = 2;
        public int DepthLogDay { get { return depth_log_day; } set { if (value != depth_log_day) { depth_log_day = value; logger.WriteMessage(0, $"DB {varname_depth_log_day} = {depth_log_day}"); } } }

        private bool connected = false;
        public bool Connected { get { return connected; } set { Debug.WriteLine($"[{DateTime.Now.ToString("hh:mm:ss.fff")}] call connected"); if (value != connected) { connected = value; logger.WriteMessage(0, $"DB connected = {connected}"); } } }

        private DataTable programs = new DataTable();
        public DataTable Programs { get; set; }

        #endregion

        #region CONSTRUCTOR

        private static DataBase instance;

        public static DataBase GetInstance()
        {
            if (instance == null)
            {
                instance = new DataBase();
            }
            return instance;
        }

        private DataBase()
        {
            MainActions();

            Thread thread = new Thread(new ThreadStart(Handler)) { IsBackground = true, Name = "Get Settings" };
            thread.Start();

            Debug.WriteLine("Start thread for database reading");
        }

        #endregion

        private void Handler()
        {
            while (true)
            {
                if (DateTime.Now.Second != 0)
                {
                    Thread.Sleep(100);
                }
                else
                {
                    MainActions();
                    Thread.Sleep(1000);
                }
            }
        }

        private void MainActions()
        {
            TestConnection();
        }

        private void TestConnection()
        {
            try
            {
                //lock (connection) lock (command)
                //{
                //    if (connection.ConnectionString != config.ConnectionString)
                //    {
                //        if (connection.State != ConnectionState.Closed)
                //            connection.Close();

                //        connection.ConnectionString = config.ConnectionString;
                //    }

                //    if (connection.State != ConnectionState.Open)
                //    {
                //        connection.Open();
                //    }

                //    if (config.DB_Type == Config.e_DB_Type.Unknown)
                //        throw new Exception("Unknow type data base. Choise next options (microsoft sql server, mysql or postgresql) in config file parameter DB_TYPE");

                //    command.Connection = (OdbcConnection)connection;
                //    command.CommandText = "select 1";
                //    command.Parameters.Clear();
                //    command.ExecuteNonQuery();
                //}
            }
            catch (Exception ex)
            {
                Connected = false;

                if (test_connection_ex == null || ex.Message != test_connection_ex.Message)
                {
                    test_connection_ex = ex;
                    logger.WriteMessage(0, "Error test connection", ex, false);
                }

                return;
            }

            Connected = true;
            return;
        }

        public void WriterDataTable(long id, DateTime timestamp, string db_type, int depth_log_day, string connection_string)
        {
            try
            {
                string sql = String.Empty;

                //if (!connected)
                //    return;

                //switch (config.DB_Type)
                //{
                //    case Config.e_DB_Type.Microsoft_SQL_Server:
                //        {
                //            sql = $@"
                //    	DECLARE @id bigint,  @timestamp datetime,  @db_type nvarchar(max), @depth_log_day, @connection_string nvarchar(max);
                //     SET @id = ?;
                //        SET @timestamp = ?; 
                //        SET @db_type = ?;
                //        SET @depth_log_day = ?;
                //        SET @connection_string = ?;

                //       INSERT INTO[1C] (@id,  @timestamp,  @db_type, @depth_log_day, @connection_string)
                //       VALUES
                //       (@id,  @timestamp,  @db_type, @depth_log_day, @connection_string)";

                //            command.CommandText = sql;

                //            command.Parameters.Add("@program_id", OdbcType.BigInt).Value = id;
                //            command.Parameters.Add("@timestamp", OdbcType.DateTime).Value = timestamp.ToString("yyyy-MM-dd hh:mm:ss");
                //            command.Parameters.Add("@description", OdbcType.NVarChar).Value = db_type;
                //            command.Parameters.Add("@description", OdbcType.BigInt).Value = depth_log_day;
                //            command.Parameters.Add("@description", OdbcType.NVarChar).Value = connection_string;

                //            break;
                //        }
                //    case Config.e_DB_Type.MySQL:
                //        {


                //            sql = $@"INSERT 1C (@id,  @timestamp,  @db_type, @depth_log_day, @connection_string) VALUES({id},'{timestamp.ToString("yyyy-MM-dd hh:mm:ss")}', ?, ?,?,?)";
                //            command.CommandText = sql;

                //            command.Parameters.Add("@program_id", OdbcType.BigInt).Value = id;
                //            command.Parameters.Add("@timestamp", OdbcType.DateTime).Value = timestamp.ToString("yyyy-MM-dd hh:mm:ss");
                //            command.Parameters.Add("@description", OdbcType.NVarChar).Value = db_type;
                //            command.Parameters.Add("@description", OdbcType.BigInt).Value = depth_log_day;
                //            command.Parameters.Add("@description", OdbcType.NVarChar).Value = connection_string;

                //            break;
                //        }
                //    default:
                //    {
                //        break;
                //    }
                //}

                //command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TestConnection();
                logger.WriteMessage(0, $"Error write to db {ex}");
            }
        }
    }
}
