using MySql.Data.MySqlClient;
using Npgsql;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Ubiety.Dns.Core.Records.NotUsed;

namespace Lib
{
    public class Database
    {

        #region STRUCTURES

        public struct SExtProp
        {
            public OdbcType data_type;
            public uint size;
            public bool primary_key;
            public bool not_null;
            public bool auto_increment;
            public bool ignore;
        }

        #endregion

        #region ENUMS

        private enum EDriver
        {
            mysql = 0,
            sqlsrv = 1,
            pgsql = 2,
            unknown = 3
        }

        #endregion

        #region CONSTANTS

        public const string default_driver = "mysql";
        public const string default_host = "127.0.0.1";
        public const int default_port = 3306;
        public const string default_charset = "utf8mb4";
        public const string default_base_name = "CORE";
        public const string default_user = "mesone";
        public const string default_password = "Mesone1_p@$$word";

        #endregion

        #region VARIABLE

        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private Parameter<string> driver;
        private Parameter<string> host;
        private Parameter<int> port;
        private Parameter<string> charset;
        private Parameter<string> base_name;
        private Parameter<string> user;
        private Parameter<string> password;

        private IDbConnection connection;
        private SqlKata.Compilers.Compiler compiler;
        private QueryFactory db;

        private string title = "";

        #endregion

        #region CONSTRUCTOR


        public Database(Parameter<string> driver,
                        Parameter<string> host,
                        Parameter<int> port,
                        Parameter<string> charset,
                        Parameter<string> base_name,
                        Parameter<string> user,
                        Parameter<string> password)
        {
            try
            {
                this.driver = driver;
                this.host = host;
                this.port = port;
                this.charset = charset;
                this.base_name = base_name;
                this.user = user;
                this.password = password;

                this.driver.ValueChanged += UpdateSettings;
                this.host.ValueChanged += UpdateSettings;
                this.port.ValueChanged += UpdateSettings;
                this.charset.ValueChanged += UpdateSettings;
                this.base_name.ValueChanged += UpdateSettings;
                this.user.ValueChanged += UpdateSettings;
                this.password.ValueChanged += UpdateSettings;

                UpdateSettings(null);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        #endregion

        #region PUBLICS

        public IEnumerable<T> Read<T>(string table_name)
        {

            IEnumerable<T> result = null;

            try
            {
                if (db != null)
                {
                    lock (db)
                    {
                        result = db.Query(table_name).Get<T>();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{title}. Read table {table_name}");
            }

            return result;
        }

        public bool Update<T>(Func<string> func_table_namer, T data)
        {
            string table_name = "";

            try
            {
                if (func_table_namer != null)
                    table_name = func_table_namer();

                return (Update<T>(table_name, data));
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{title}. Update in table {table_name}");
            }

            return false;
        }

        public bool Update<T>(string table_name, T data)
        {

            bool result = false;

            try
            {
                if (db != null)
                {
                    lock (db)
                    {

                        Dictionary<string, object> condition = new Dictionary<string, object>();
                        Dictionary<string, object> values = new Dictionary<string, object>();
                        foreach (PropertyInfo prop in data.GetType().GetProperties().Where(x => x.GetCustomAttribute(typeof(Field)) != null))
                        {
                            Field attr = prop.GetCustomAttribute(typeof(Field)) as Field;
                            if (attr != null && (attr.PK || attr.UQ))
                                condition.Add(prop.Name, prop.GetValue(data));
                            else
                                values.Add(prop.Name, prop.GetValue(data));
                        }

                        if (condition.Count > 0)
                        {
                            if (db.Query(table_name).Where(condition).Update(values) == 0) Insert<T>(table_name, data);
                        }
                        else
                        {
                            if (db.Query(table_name).Update(values) == 0) Insert<T>(table_name, data);
                        }
                    }
                }

                result = true;

            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{title}. Update in table {table_name}");
            }

            return result;
        }

        public bool Insert<T>(string table_name, T data)
        {
            bool result = false;

            try
            {
                if (db != null)
                {
                    lock (db)
                    {
                        db.Query(table_name).Insert(data);
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{title}. Insert to table {table_name}");
            }

            return result;
        }

        public bool CheckExistTable(string table_name)
        {
            bool result = false;

            try
            {
                if (db != null)
                {
                    lock (db)
                    {
                        string sql = string.Empty;

                        if (connection.GetType().Equals(typeof(SqlConnection)))
                        {
                            throw new Exception("Create code for handling MSSQL");
                        }
                        else if (connection.GetType().Equals(typeof(MySqlConnection)))
                        {
                            IEnumerable<dynamic> result_query = db.Query("information_schema.tables").Where("Table_name", table_name).Where("Table_schema", connection.Database).Get();
                            result = result_query.Count() != 0;

                        }
                        else if (connection.GetType().Equals(typeof(NpgsqlConnection)))
                        {
                            throw new Exception("Create code for handling PostgreSQL");
                        }
                        else
                        {
                            throw new Exception("Don't know database type");
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{title}. Check exist table {table_name}");
            }

            return result;
        }

        public bool CreateTable<T>(string table_name)
        {

            bool result = false;

            try
            {

                if (db != null)
                {
                    lock (db)
                    {
                        string sql = string.Empty;

                        if (connection.GetType().Equals(typeof(SqlConnection)))
                        {
                            throw new Exception("Create code for handling MSSQL");
                        }
                        else if (connection.GetType().Equals(typeof(MySqlConnection)))
                        {
                            sql = $"CREATE TABLE `{table_name}` ( ";

                            foreach (PropertyInfo prop in typeof(T).GetProperties().Where(x => x.GetCustomAttribute(typeof(Field)) != null))
                            {
                                Field attr = prop.GetCustomAttribute(typeof(Field)) as Field;
                                if (!attr.IGNORE)
                                {

                                    sql += $" `{prop.Name}` ";

                                    sql += $" {attr.TYPE}";
                                    if (attr.SIZE != 0) sql += $"({attr.SIZE}) ";
                                    if (attr.UN) sql += $" UNSIGNED ";
                                    if (attr.AI) sql += $" AUTO_INCREMENT ";
                                    if (attr.PK) sql += $" PRIMARY KEY ";
                                    if (attr.NN) sql += $" NOT NULL ";
                                    sql += ",";
                                    if (attr.UQ)
                                    {
                                        sql += $" UNIQUE (`{prop.Name}`),";
                                    }
                                }
                            }
                            sql = sql.Substring(0, sql.Length - 1);
                            sql += ");";

                        }
                        else if (connection.GetType().Equals(typeof(NpgsqlConnection)))
                        {
                            throw new Exception("Create code for handling PostgreSQL");
                        }
                        else
                        {
                            throw new Exception("Don't know database type");
                        }

                        db.Statement(sql);
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{title}. Create table {table_name}");
            }

            return result;

        }

        #endregion

        #region PRIVATES

        private void UpdateSettings(object value)
        {
            try
            {
                switch (driver.Value)
                {
                    case "sqlsrv":
                        throw new Exception("Create code for handling MSSQL");
                        connection = new SqlConnection();
                        compiler = new SqlKata.Compilers.SqlServerCompiler();
                        break;
                    case "mysql":
                        connection = new MySqlConnection($"Server={host.Value};Port={port.Value};CharSet={charset.Value};Database={base_name.Value};Uid={user.Value};Pwd={password.Value};");
                        compiler = new SqlKata.Compilers.MySqlCompiler();
                        break;
                    case "pgsql":
                        throw new Exception("Create code for handling PostgreSQL");
                        connection = new NpgsqlConnection();
                        compiler = new SqlKata.Compilers.PostgresCompiler();
                        break;
                    default:
                        connection = null;
                        compiler = null;
                        break;
                }

                if (connection != null && compiler != null)
                {
                    db = new QueryFactory(connection, compiler);
                    title = $"Databse {db.Connection.Database}";
                }
                else
                    db = null;

            }
            catch (Exception ex)
            {
                logger.Error(ex, "update settings");
            }
        }

        #endregion

    }
}
