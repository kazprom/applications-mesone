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

namespace Lib
{
    public class CDatabase : CChild
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

        public enum EPropKeys
        {
            Driver,
            Host,
            Port,
            Charset,
            BaseName,
            User,
            Password
        }

        public enum EState
        {
            Disconnected,
            Connected
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

        private IDbConnection connection;
        private SqlKata.Compilers.Compiler compiler;
        private QueryFactory db;

        #endregion

        #region PROPERTIES

        public EState State { get { return db != null ? EState.Connected : EState.Disconnected; } }

        public Dictionary<string, string> Props { get; set; } = new Dictionary<string, string>();

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
                if (Logger != null) Logger.Error(ex, $"Table {table_name}");
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
                if (Logger != null) Logger.Error(ex, $"Table {table_name}");
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
                if (Logger != null) Logger.Error(ex, $"Table {table_name}");
            }

            return result;
        }

        public bool Update(string table_name, Dictionary<string, object> constraints, Dictionary<string, object> values, bool auto_insert = true)
        {
            bool result = false;

            try
            {
                if (db != null && constraints != null && values != null)
                {
                    lock (db)
                    {
                        if (constraints.Count > 0)
                            result = db.Query(table_name).Where(constraints).Update(values) != 0;
                        else
                            result = db.Query(table_name).Update(values) != 0;

                        //Insert(table_name, values);

                        if (!result && auto_insert)
                            result = Insert(table_name, constraints.Concat(values).ToDictionary(x => x.Key, x => x.Value));

                    }
                }


            }
            catch (Exception ex)
            {
                if (Logger != null) Logger.Error(ex, $"Table {table_name}");
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

                        Update(table_name, constraints, values);

                        /*
                        if (constraints.Count > 0)
                        {
                            if (db.Query(table_name).Where(constraints).Update(values) == 0) Insert<T>(table_name, data);
                        }
                        else
                        {
                            if (db.Query(table_name).Update(values) == 0) Insert<T>(table_name, data);
                        }
                        */
                    }
                }

                result = true;

            }
            catch (Exception ex)
            {
                if (Logger != null) Logger.Error(ex, $"Table {table_name}");
            }

            return result;
        }

        public bool Insert(string table_name, Dictionary<string, object> data)
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
                if (Logger != null) Logger.Error(ex, $"Table {table_name}");
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
                if (Logger != null) Logger.Error(ex, $"Table {table_name}");
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
                if (Logger != null) Logger.Error(ex, $"Table {table_name}");
            }

            return result;
        }

        public bool? CompareTableSchema<T>(string table_name)
        {
            return CompareTableSchema(table_name,
                                      typeof(T).
                                      GetProperties().
                                      Where(x => x.GetCustomAttribute(typeof(Field)) != null).
                                      ToDictionary(y => y.Name, y => (Field)y.GetCustomAttribute(typeof(Field))));
        }

        public bool? CompareTableSchema(string table_name, Dictionary<string, Field> fields)
        {
            bool? result = null;

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
                            IEnumerable<Tables.MySQL.CInfoSchemaCols> rows;

                            rows = WhereRead<Tables.MySQL.CInfoSchemaCols>(Tables.MySQL.CInfoSchemaCols.TableName, new
                            {
                                Table_schema = db.Connection.Database,
                                Table_name = table_name
                            });

                            if (rows != null)
                            {

                                result = true;

                                foreach (var field in fields)
                                {
                                    Tables.MySQL.CInfoSchemaCols row = rows.Where(x => x.Column_name.Equals(field.Key, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                                    if (row == null)
                                    {
                                        Logger.Warn($"Table [{table_name}] Column [{field.Key}] not found");
                                        result = false;
                                    }
                                    else
                                    {
                                        string type = field.Value.GetType_MySQL();

                                        if (!row.Column_type.Equals(type, StringComparison.OrdinalIgnoreCase))
                                        {
                                            Logger.Warn($"Table [{table_name}] Column [{field.Key}] Type [{type}] wrong. Current [{row.Column_type}]");
                                            result = false;
                                        }

                                        if (field.Value.PK != row.Column_key.Contains("PRI"))
                                        {
                                            Logger.Warn($"Table [{table_name}] Column [{field.Key}] PRIMARY[{field.Value.PK}] wrong. Current [{row.Column_key}]");
                                            result = false;
                                        }

                                        if (field.Value.UQ != row.Column_key.Contains("UNI"))
                                        {
                                            Logger.Warn($"Table [{table_name}] Column [{field.Key}] UNIQUE[{field.Value.UQ}] wrong. Current [{row.Column_key}]");
                                            result = false;
                                        }

                                        if (field.Value.AI != row.Extra.Contains("auto_increment"))
                                        {
                                            Logger.Warn($"Table [{table_name}] Column [{field.Key}] AUTO_INCREMENT[{field.Value.AI}] wrong. Current [{row.Extra}]");
                                            result = false;
                                        }

                                        if (field.Value.NN != row.Is_nullable.Equals("NO"))
                                        {
                                            Logger.Warn($"Table [{table_name}] Column [{field.Key}] NOT NULL[{field.Value.NN}] wrong. Current IS_NULLABLE [{row.Is_nullable}]");
                                            result = false;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                Logger.Warn($"Can't get data from table {Tables.MySQL.CInfoSchemaCols.TableName} for table {table_name}");
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
                if (Logger != null) Logger.Error(ex, $"Table {table_name}");
            }

            return result;
        }

        public bool? CheckExistTable(string table_name)
        {
            bool? result = null;

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
                            IEnumerable<dynamic> result_query = db.Query(Tables.MySQL.CInfoSchemaTabs.TableName).
                                                                   Where("Table_name", table_name).
                                                                   Where("Table_schema", connection.Database).
                                                                   Get();

                            if (result_query.Count() != 0)
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                                //logger.Warn($"Table {table_name} is absent");
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
                if (Logger != null) Logger.Error(ex, $"Table {table_name}");
            }

            return result;
        }

        public bool? CheckTable<T>(string table_name)
        {
            try
            {
                switch (CheckExistTable(table_name))
                {
                    case null:
                        return null;
                    case false:
                        Logger.Warn($"Table {table_name} is absent");
                        return false;
                }

                switch (CompareTableSchema<T>(table_name))
                {
                    case null:
                        return null;
                    case false:
                        Logger.Warn($"Table {table_name} has wrong scheme");
                        return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                if (Logger != null) Logger.Error(ex);
                return null;
            }
        }

        public bool CreateTable<T>(string table_name)
        {

            return CreateTable(table_name,
                                typeof(T).
                                GetProperties().
                                Where(x => x.GetCustomAttribute(typeof(Field)) != null).
                                ToDictionary(y => y.Name, y => (Field)y.GetCustomAttribute(typeof(Field))));
        }

        public bool CreateTable(string table_name, Dictionary<string, Field> fields)
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

                            foreach (var field in fields)
                            {

                                sql += $" `{field.Key}` ";

                                sql += $" {field.Value.GetType_MySQL()}";
                                if (field.Value.AI) sql += $" AUTO_INCREMENT ";
                                if (field.Value.PK) sql += $" PRIMARY KEY ";
                                if (field.Value.NN) sql += $" NOT NULL ";
                                sql += ",";
                                if (field.Value.UQ)
                                {
                                    sql += $" UNIQUE (`{field.Key}`),";
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
                if (Logger != null) Logger.Error(ex, $"Table {table_name}");
            }

            return result;
        }

        public bool ClearTable(string table_name)
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
                            sql = $"DELETE FROM `{table_name}`";
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
                if (Logger != null) Logger.Error(ex, $"Table {table_name}");
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
                if (Logger != null) Logger.Error(ex, $"Table {table_name}");
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
                            IEnumerable<Tables.MySQL.CInfoSchemaTabs> rows;

                            rows = WhereLikeRead<Tables.MySQL.CInfoSchemaTabs>(Tables.MySQL.CInfoSchemaTabs.TableName,
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

                if (Logger != null) Logger.Error(ex);

            }

            return result;
        }

        public void LoadSettings(Dictionary<string, string> props)
        {
            try
            {
                bool reconnect = false;

                foreach (var key in Enum.GetValues(typeof(EPropKeys)))
                {
                    if (props.ContainsKey(key.ToString()))
                    {
                        if (!Props.ContainsKey(key.ToString()))
                            Props.Add(key.ToString(), null);

                        if (Props[key.ToString()] != (props[key.ToString()]))
                        {
                            Props[key.ToString()] = props[key.ToString()];
                            reconnect = true;
                        }
                    }
                    else
                    {
                        Logger.Error($"Properties don't contain key {key}");
                    }

                }

                if (reconnect)
                {


                    switch (Props[EPropKeys.Driver.ToString()])
                    {
                        case "sqlsrv":
                            throw new Exception("Create code for handling MSSQL");
                            connection = new SqlConnection();
                            compiler = new SqlKata.Compilers.SqlServerCompiler();
                            break;
                        case "mysql":
                            connection = new MySqlConnection($"Server={Props[EPropKeys.Host.ToString()]};" +
                                                             $"Port={Props[EPropKeys.Port.ToString()]};" +
                                                             $"CharSet={Props[EPropKeys.Charset.ToString()]};" +
                                                             $"Database={Props[EPropKeys.BaseName.ToString()]};" +
                                                             $"Uid={Props[EPropKeys.User.ToString()]};" +
                                                             $"Pwd={Props[EPropKeys.Password.ToString()]};");
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
                        Logger.Info($"Connection {connection.ConnectionString}");
                    }
                    else
                        db = null;

                }

            }
            catch (Exception ex)
            {
                if (Logger != null) Logger.Error(ex);
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
                        if (!attr.AI)
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
                if (Logger != null) Logger.Error(ex);
            }

        }

        #endregion

    }
}
