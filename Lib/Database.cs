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
//using Ubiety.Dns.Core.Records.NotUsed;

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

        private NLog.Logger logger;


        private IDbConnection connection;
        private SqlKata.Compilers.Compiler compiler;
        private QueryFactory db;


        #endregion

        #region PROPERTIES

        public string Title { get; private set; }

        public ulong ID { get; private set; }

        public string Name { get; private set; }

        public string Driver { get; private set; }

        public string Host { get; private set; }

        public uint Port { get; private set; }

        public string Charset { get; private set; }

        public string BaseName { get; private set; }

        public string User { get; private set; }

        public string Password { get; private set; }

        #endregion

        #region CONSTRUCTOR

        public Database(ulong id, NLog.Logger logger = null)
        {
            ID = id;

            if (logger == null)
                this.logger = NLog.LogManager.GetCurrentClassLogger();
            else
                this.logger = logger;

        }

        #endregion

        #region PUBLICS

        public IEnumerable<T> WhereLikeRead<T>(string table_name, object constraints, string column, string value)
        {
            IEnumerable<T> result = null;

            try
            {
                if (db != null)
                {
                    lock (db)
                    {
                        result = db.Query(table_name).Where(constraints).WhereLike(column, value).Get<T>();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Title}. Read where like table {table_name}");
            }

            return result;
        }

        public IEnumerable<T> WhereRead<T>(string table_name, object constraints)
        {
            IEnumerable<T> result = null;

            try
            {
                if (db != null)
                {
                    lock (db)
                    {
                        result = db.Query(table_name).Where(constraints).Get<T>();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Title}. Read where table {table_name}");
            }

            return result;
        }

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
                logger.Error(ex, $"{Title}. Read table {table_name}");
            }

            return result;
        }

        public bool Update<T>(string table_name, T data)
        {

            bool result = false;

            try
            {
                if (db != null && data != null)
                {
                    lock (db)
                    {

                        Dictionary<string, object> constraints = new Dictionary<string, object>();
                        Dictionary<string, object> values = new Dictionary<string, object>();
                        Constraints<T>(data, ref constraints, ref values);

                        if (constraints.Count > 0)
                        {
                            if (db.Query(table_name).Where(constraints).Update(values) == 0) Insert<T>(table_name, data);
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
                logger.Error(ex, $"{Title}. Update in table {table_name}");
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
                logger.Error(ex, $"{Title}. Insert to table {table_name}");
            }

            return result;
        }

        public bool WhereNotInDelete<T>(string table_name, string col_name, T[] data)
        {
            bool result = false;
            try
            {

                if (db != null)
                {
                    lock (db)
                    {
                        db.Query(table_name).WhereNotIn(col_name, data).Delete();
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Title}. Delete where NOT in table {table_name}");
            }

            return result;
        }

        public bool CompareTableSchema<T>(string table_name)
        {
            bool result = true;

            try
            {
                if (db != null)
                {
                    lock (db)
                    {

                        if (connection.GetType().Equals(typeof(SqlConnection)))
                        {
                            throw new Exception("Create code for handling MSSQL");
                        }
                        else if (connection.GetType().Equals(typeof(MySqlConnection)))
                        {
                            IEnumerable<Structs.MySQLInfoSchemaCols> rows;

                            rows = WhereRead<Structs.MySQLInfoSchemaCols>("INFORMATION_SCHEMA.COLUMNS", new
                            {
                                Table_schema = db.Connection.Database,
                                Table_name = table_name
                            });

                            if (rows != null)
                            {
                                foreach (PropertyInfo prop in typeof(T).GetProperties().Where(x => x.GetCustomAttribute(typeof(Field)) != null))
                                {

                                    Structs.MySQLInfoSchemaCols row = rows.Where(x => x.Column_name.Equals(prop.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                                    if (row == null)
                                    {
                                        logger.Warn($"{Title}. Table [{table_name}] Column [{prop.Name}] not found");
                                        result = false;
                                    }
                                    else
                                    {
                                        Field attr = prop.GetCustomAttribute(typeof(Field)) as Field;

                                        string type = $"{attr.TYPE}";
                                        type += attr.SIZE > 0 ? $"({attr.SIZE})" : "";
                                        type += attr.UN ? " unsigned" : "";

                                        if (!row.Column_type.Equals(type, StringComparison.OrdinalIgnoreCase))
                                        {
                                            logger.Warn($"{Title}. Table [{table_name}] Column [{prop.Name}] Type [{type}] wrong. Current [{row.Column_type}]");
                                            result = false;
                                        }

                                        if (attr.PK != row.Column_key.Contains("PRI"))
                                        {
                                            logger.Warn($"{Title}. Table [{table_name}] Column [{prop.Name}] PRIMARY[{attr.PK}] wrong. Current [{row.Column_key}]");
                                            result = false;
                                        }

                                        if (attr.UQ != row.Column_key.Contains("UNI"))
                                        {
                                            logger.Warn($"{Title}. Table [{table_name}] Column [{prop.Name}] UNIQUE[{attr.UQ}] wrong. Current [{row.Column_key}]");
                                            result = false;
                                        }

                                        if (attr.AI != row.Extra.Contains("auto_increment"))
                                        {
                                            logger.Warn($"{Title}. Table [{table_name}] Column [{prop.Name}] AUTO_INCREMENT[{attr.AI}] wrong. Current [{row.Extra}]");
                                            result = false;
                                        }

                                        if (attr.NN != row.Is_nullable.Equals("NO"))
                                        {
                                            logger.Warn($"{Title}. Table [{table_name}] Column [{prop.Name}] NOT NULL[{attr.NN}] wrong. Current IS_NULLABLE [{row.Is_nullable}]");
                                            result = false;
                                        }

                                    }
                                }
                            }
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
                logger.Error(ex, $"{Title}. Comare table schema {table_name}");
                result = false;
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
                logger.Error(ex, $"{Title}. Check exist table {table_name}");
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
                logger.Error(ex, $"{Title}. Create table {table_name}");
            }

            return result;

        } 
        
        public bool RemoveTable(string table_name)
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
                            sql = $"DROP TABLE `{table_name}`";
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
                logger.Error(ex, $"{Title}. Remove table {table_name}");
            }

            return result;

        }

        public IEnumerable<string> GetListTables(string filter)
        {
            IEnumerable<string> result = null;

            try
            {

                if (db != null)
                {
                    lock (db)
                    {

                        if (connection.GetType().Equals(typeof(SqlConnection)))
                        {
                            throw new Exception("Create code for handling MSSQL");
                        }
                        else if (connection.GetType().Equals(typeof(MySqlConnection)))
                        {
                            IEnumerable<Structs.MySQLInfoSchemaTabs> rows;

                            rows = WhereLikeRead<Structs.MySQLInfoSchemaTabs>("INFORMATION_SCHEMA.TABLES", 
                                                                              new { Table_schema = db.Connection.Database },
                                                                              "table_name", filter);

                            result = rows.Select(x => x.Table_name);

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

                logger.Error(ex, $"{Title}. Get list tables");

            }

            return result;
        }

        public void LoadSettings(string name, string driver, string host, uint port, string charset, string base_name, string user, string password)
        {
            try
            {
                Name = name;
                Title = $"Database {Name}";

                if (Driver != driver || Host != host || Port != port || Charset != charset || BaseName != base_name || User != user || Password != password)
                {

                    Driver = driver;
                    Host = host;
                    Port = port;
                    Charset = charset;
                    BaseName = base_name;
                    User = user;
                    Password = password;

                    switch (driver)
                    {
                        case "sqlsrv":
                            throw new Exception("Create code for handling MSSQL");
                            connection = new SqlConnection();
                            compiler = new SqlKata.Compilers.SqlServerCompiler();
                            break;
                        case "mysql":
                            connection = new MySqlConnection($"Server={host};Port={port};CharSet={charset};Database={base_name};Uid={user};Pwd={password};");
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
                        logger.Info($"{Title}. Connection {connection.ConnectionString}");
                    }
                    else
                        db = null;

                }

            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Title}. Settings");
            }
        }

        #endregion

        #region PRIVATES


        private void Constraints<T>(T data, ref Dictionary<string, object> constraints, ref Dictionary<string, object> values)
        {
            try
            {
                IEnumerable<PropertyInfo> props = data.GetType().GetProperties().Where(x => x.GetCustomAttribute(typeof(Field)) != null);
                foreach (PropertyInfo prop in props)
                {
                    Field attr = prop.GetCustomAttribute(typeof(Field)) as Field;
                    if (attr != null)
                    {
                        if (!attr.AI && !attr.IGNORE)
                        {
                            if (attr.PK || attr.UQ)
                            {
                                if (constraints != null)
                                    constraints.Add(prop.Name, prop.GetValue(data));
                            }
                            else
                            {
                                if (values != null)
                                    values.Add(prop.Name, prop.GetValue(data));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Title}. Constraints");
            }

        }

        #endregion

    }
}
