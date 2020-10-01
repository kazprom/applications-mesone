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
            Write();
        }

        private void TestConnection()
        {
            try
            {
                lock (connection) lock (command)
                    {
                        if (connection.ConnectionString != config.ConnectionString)
                        {
                            if (connection.State != ConnectionState.Closed)
                                connection.Close();

                            connection.ConnectionString = config.ConnectionString;
                        }

                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }

                        if (config.DB_Type == Config.e_DB_Type.Unknown)
                            throw new Exception("Unknow type data base. Choise next options (microsoft sql server, mysql or postgresql) in config file parameter DB_TYPE");

                        command.Connection = (OdbcConnection)connection;
                        command.CommandText = "select 1";
                        command.Parameters.Clear();
                        command.ExecuteNonQuery();
                    }
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

        public void Write()
        {
            try
            {
             
                if (config.DataTable.Rows.Count > 0)
                {

                    var table = config.DataTable;

                    string sql = String.Empty;

                    if (!connected)
                        return;

                    switch (config.DB_Type)
                    {
                        case Config.e_DB_Type.Microsoft_SQL_Server:
                        {

                            //IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
                            //WHERE TABLE_NAME = '1C_xml_test' AND COLUMN_NAME = 'name' AND COLUMN_NAME = 'command' AND COLUMN_NAME = 'input'AND COLUMN_NAME = 'output')
                            //BEGIN
                            //    -- do something, e.g.
                            //    -- ALTER TABLE TEST ADD PRICE DECIMAL
                            //END




                            //       sql = $@"
                            //DECLARE @id bigint,  @timestamp datetime,  @db_type nvarchar(max), @depth_log_day, @connection_string nvarchar(max);
                            //SET @id = ?;
                            //   SET @timestamp = ?; 
                            //   SET @db_type = ?;
                            //   SET @depth_log_day = ?;
                            //   SET @connection_string = ?;

                                //  INSERT INTO[1C] (@id,  @timestamp,  @db_type, @depth_log_day, @connection_string)
                                //  VALUES
                                //  (@id,  @timestamp,  @db_type, @depth_log_day, @connection_string)";

                                //       command.CommandText = sql;

                                //       command.Parameters.Add("@program_id", OdbcType.BigInt).Value = id;
                                //       command.Parameters.Add("@timestamp", OdbcType.DateTime).Value = timestamp.ToString("yyyy-MM-dd hh:mm:ss");
                                //       command.Parameters.Add("@description", OdbcType.NVarChar).Value = db_type;
                                //       command.Parameters.Add("@description", OdbcType.BigInt).Value = depth_log_day;
                                //       command.Parameters.Add("@description", OdbcType.NVarChar).Value = connection_string;

                                break;
                        }
                        case Config.e_DB_Type.MySQL:
                        {

                            //ALTER TABLE 1C_xml_test ADD COLUMN `name` TEXT,
                            //ADD COLUMN  command TEXT,
                            //    ADD COLUMN input TEXT,
                            //    ADD COLUMN  output text

                            sql = "SELECT `name`, `command`, `input`, `output` from 1C_xml_test limit 1";
                            command.CommandText = sql;
                            var count= command.ExecuteScalar();//.ExecuteNonQuery();

                            //if (count > 0)
                            //{
                            //    sql = "ALTER TABLE 1C_xml_test SET ADD name text, command text, input text, output text";


                            //}
                            //    sql = "ALTER TABLE 1C_xml_test SET ADD name text, command text, input text, output text";


                            sql = $@"CREATE TABLE 1C_xml_test
                                                (
                                                    program_id bigint NOT NULL,
                                                    timestamp datetime NULL,
                                                    description LONGTEXT NULL
                                                 ) ";


                                foreach (DataRow row in table.Rows)
                            {

                                string str_columns_values = "";
                                //string table_name = "[" + row["nameOfTable"].ToString() + "] ";
                                string str_columns = "[" + table.Columns[1].ColumnName + "],";
                                string str_values = "'" + Convert.ToDateTime(row["date_time"])
                                    .ToString("yyyy-MM-dd HH:mm:ss.fff") + "',";
                                //string str_time_current = "'" + Convert.ToDateTime(row["date_time_pocket"]).ToString("yyyy-MM-dd HH:mm:ss.fff") + "',"; ;

                                for (int i = 2; i < row.ItemArray.Length; i++)
                                {
                                    if (!row.IsNull(i))
                                    {
                                        str_values += "'" + row[i].ToString().Replace(",", ".") + "',";
                                        str_columns += "[" + table.Columns[i].ColumnName.ToString() + "],";

                                        str_columns_values += "[" + table.Columns[i].ColumnName.ToString() +
                                                              "] = '" + row[i].ToString().Replace(",", ".") + "',";
                                    }
                                }
                            }



                            //sql = $@"INSERT 1C (@id,  @timestamp,  @db_type, @depth_log_day, @connection_string) VALUES({id},'{timestamp.ToString("yyyy-MM-dd hh:mm:ss")}', ?, ?,?,?)";
                                    //command.CommandText = sql;

                                    //command.Parameters.Add("@program_id", OdbcType.BigInt).Value = id;
                                    //command.Parameters.Add("@timestamp", OdbcType.DateTime).Value = timestamp.ToString("yyyy-MM-dd hh:mm:ss");
                                    //command.Parameters.Add("@description", OdbcType.NVarChar).Value = db_type;
                                    //command.Parameters.Add("@description", OdbcType.BigInt).Value = depth_log_day;
                                    //command.Parameters.Add("@description", OdbcType.NVarChar).Value = connection_string;

                                    break;
                        }
                        default:
                        {
                            break;
                        }
                    }

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TestConnection();
                logger.WriteMessage(0, $"Error write to db {ex}");
            }
        }
    }
}
