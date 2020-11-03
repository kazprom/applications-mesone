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

namespace Lib
{
    public class Database
    {

        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

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

        #endregion


        #region PROPERTIES
        /*
        public const EType default_type = EType.MySQL;
        private EType type = default_type;
        public EType Type { get { return type; } set { type = value; } }

        public const string default_connection_string = "Driver={mySQL ODBC 8.0 ANSI Driver}; Server=myServerAddress;Option=131072;Stmt=;Database=myDataBase;User=myUsername;Password=myPassword;";
        private string connection_string = default_connection_string;
        public string ConnectionString { get { return connection_string; } }
        */
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
                logger.Error(ex);
            }

            return result;
        }

        /*
        public bool Write(DataSet ds)
        {
            try
            {
                if (!TestConnection())
                    return false;

                lock (ds)
                {
                    foreach (DataTable table in ds.Tables)
                    {
                        Write(table, true, false);
                    }
                }

            }
            catch (Exception ex)
            {
                Lib.Message.Make("Error write data set", ex);
                return false;
            }

            return true;

        }
        public bool Write(DataTable dt, bool create_table, bool rewrite_row)
        {
            try
            {
                if (dt != null)
                {

                    if (!TestConnection())
                        return false;

                    lock (dt)
                    {
                        if (create_table && !TableExists(dt))
                        {
                            TableAdd(dt);
                        }


                        foreach (DataRow row in dt.Rows)
                        {
                            if (!rewrite_row || !RowExists(row))
                            {
                                RowInsert(row);
                            }
                            else
                            {
                                RowUpdate(row);
                            }

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Lib.Message.Make("Error write data table", ex);
                return false;
            }

            return true;

        }



        public string[] GetListTables(string condition = "")
        {
            try
            {
                if (!TestConnection())
                    return null;

                string sql = String.Empty;

                switch (type)
                {
                    case EType.MSSQLServer:
                    case EType.MySQL:
                    case EType.PostgreSQL:
                        {
                            sql = $@"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{connection.Database}' ";
                            if (condition != "")
                                sql += $"AND ({condition})";
                            break;
                        }
                }

                lock (command)
                {
                    DataTable dt = new DataTable();
                    using (OdbcDataAdapter adapter = new OdbcDataAdapter(sql, connection))
                    {
                        adapter.Fill(dt);
                    }
                    return dt.Rows.Cast<DataRow>().Select(x => x["TABLE_NAME"].ToString()).ToArray();
                }
            }
            catch (Exception ex)
            {
                Lib.Message.Make("Error get list tables", ex);
                return null;
            }
        }
        public bool DeleteTables(string[] table_names)
        {
            try
            {
                if (!TestConnection())
                    return false;

                foreach (string table_name in table_names)
                {
                    DeleteTable(table_name);
                }
            }
            catch (Exception ex)
            {
                Lib.Message.Make("Error delete tables", ex);
                return false;
            }

            return true;
        }

        public bool DeleteTable(string table_name)
        {

            try
            {
                if (!TestConnection())
                    return false;

                string sql = String.Empty;

                switch (type)
                {
                    case EType.MSSQLServer:
                    case EType.MySQL:
                    case EType.PostgreSQL:
                        {
                            sql = $@"DROP TABLE '{table_name}' ";
                            break;
                        }
                }

                lock (command)
                {
                    command.Parameters.Clear();
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Lib.Message.Make($"Error delete table {table_name}", ex);
                return false;
            }

            return true;

        }
        */
        #endregion

        #region PRIVATES

        private void UpdateSettings(object value)
        {
            try
            {
                switch (driver.Value)
                {
                    case "sqlsrv":
                        connection = new SqlConnection();
                        compiler = new SqlKata.Compilers.SqlServerCompiler();
                        break;
                    case "mysql":
                        connection = new MySqlConnection($"Server={host.Value};Port={port.Value};CharSet={charset.Value};Database={base_name.Value};Uid={user.Value};Pwd={password.Value};");
                        compiler = new SqlKata.Compilers.MySqlCompiler();
                        break;
                    case "pgsql":
                        connection = new NpgsqlConnection();
                        compiler = new SqlKata.Compilers.PostgresCompiler();
                        break;
                    default:
                        connection = null;
                        compiler = null;
                        break;
                }

                if (connection != null && compiler != null)
                    db = new QueryFactory(connection, compiler);
                else
                    db = null;

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /*
        private bool TestConnection()
        {
            try
            {
                lock (connection) lock (command)
                    {
                        if (connection.ConnectionString != connection_string)
                        {
                            if (connection.State != ConnectionState.Closed)
                                connection.Close();

                            connection.ConnectionString = connection_string;
                        }

                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }

                        if (type == EType.Unknown)
                            throw new Exception($"Unknow type data base. Choise next options {string.Join(" , ", Enum.GetValues(typeof(EType)).Cast<EType>().Where(x => x != EType.Unknown).Select(x => x.ToString()).ToArray())}");

                        command.Connection = connection;
                        command.CommandText = "SELECT 1";
                        command.Parameters.Clear();
                        command.ExecuteNonQuery();
                    }
            }
            catch (Exception ex)
            {
                Lib.Message.Make("Error connection", ex);
                return false;

            }

            return true;

        }

        private bool TableExists(DataTable dt)
        {
            bool result = false;

            try
            {
                string sql = String.Empty;

                switch (type)
                {
                    case EType.MSSQLServer:
                    case EType.MySQL:
                    case EType.PostgreSQL:
                        {
                            sql = $@"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{connection.Database}' AND  TABLE_NAME = '{dt.TableName}'";
                            break;
                        }
                }

                lock (command)
                {
                    command.Parameters.Clear();
                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        result = reader.HasRows;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error to check table exists", ex);
            }

            if (!result)
                Message.Make($"Error in database. Database doesn't have table [{dt.TableName}].");

            return result;
        }

        private bool TableAdd(DataTable dt)
        {
            try
            {

                string sql = string.Empty;
                List<string> props = new List<string>();

                switch (type)
                {
                    case EType.MSSQLServer:
                        {
                            foreach (DataColumn col in dt.Columns)
                            {
                                List<string> result = new List<string>();
                                if (col.ExtendedProperties.ContainsKey(typeof(SExtProp)))
                                {
                                    SExtProp ext_prop = (SExtProp)col.ExtendedProperties[typeof(SExtProp)];
                                    result.Add(ext_prop.data_type.ToString());
                                    if (ext_prop.size > 0) result.Add($"({ext_prop.size})");
                                    if (ext_prop.auto_increment) result.Add("IDENTITY(1,1)");
                                    if (ext_prop.primary_key) result.Add("PRIMARY KEY");
                                    if (ext_prop.not_null) result.Add("NOT NULL"); else result.Add("NULL");
                                }
                                props.Add($"[{col}] { string.Join(" ", result.ToArray())} ");
                            }

                            sql = $@"CREATE TABLE [{dt.TableName}] ({string.Join(" , ", props)})";
                            break;
                        }
                    case EType.MySQL:
                    case EType.PostgreSQL:
                        {
                            foreach (DataColumn col in dt.Columns)
                            {
                                List<string> result = new List<string>();
                                if (col.ExtendedProperties.ContainsKey(typeof(SExtProp)))
                                {
                                    SExtProp ext_prop = (SExtProp)col.ExtendedProperties[typeof(SExtProp)];
                                    result.Add(ext_prop.data_type.ToString());
                                    if (ext_prop.size > 0) result.Add($"({ext_prop.size})");
                                    if (ext_prop.auto_increment) result.Add("AUTO_INCREMENT");
                                    if (ext_prop.primary_key) result.Add("PRIMARY KEY");
                                    if (ext_prop.not_null) result.Add("NOT NULL"); else result.Add("NULL");
                                }
                                props.Add($"`{col}` { string.Join(" ", result.ToArray())} ");
                            }
                            sql = $@"CREATE TABLE `{dt.TableName}` ({string.Join(" , ", props)})";
                            break;

                        }
                }

                lock (command)
                {

                    command.Parameters.Clear();
                    command.CommandText = sql;
                    command.ExecuteNonQuery();

                }

            }
            catch (Exception ex)
            {
                Lib.Message.Make($"Error add table", ex);
                return false;
            }

            return true;

        }

        private bool TableCheckScheme(DataTable dt)
        {

            bool error = false;

            try
            {

                string sql = string.Empty;

                string col_name_column_name = "COLUMN_NAME";
                string col_name_data_type = "DATA_TYPE";
                string col_name_character_maximum_length = "CHARACTER_MAXIMUM_LENGTH";
                string col_name_is_nullable = "IS_NULLABLE";

                switch (type)
                {
                    case EType.MSSQLServer:
                    case EType.MySQL:
                    case EType.PostgreSQL:
                        {
                            sql = $"SELECT {col_name_column_name}, {col_name_data_type}, {col_name_character_maximum_length} , {col_name_is_nullable} " +
                                  $"FROM INFORMATION_SCHEMA.COLUMNS " +
                                  $"WHERE " +
                                  $"TABLE_SCHEMA = '{connection.Database}' " +
                                  $"AND " +
                                  $"TABLE_NAME = '{dt.TableName}'";
                            break;
                        }
                }

                DataTable scheme = new DataTable();

                lock (command)
                {
                    using (OdbcDataAdapter adapter = new OdbcDataAdapter(sql, connection))
                    {
                        adapter.Fill(scheme);
                    }
                }


                foreach (DataColumn col in dt.Columns)
                {

                    if (!col.ExtendedProperties.ContainsKey(typeof(Lib.Database.SExtProp)))
                    {
                        Lib.Message.Make($"Error in configuration of data table [{dt.TableName}]. Column [{col.ColumnName}] doesn't contain extended properties.");
                        error = true;
                    }
                    else
                    {
                        Lib.Database.SExtProp prop = (SExtProp)col.ExtendedProperties[typeof(Lib.Database.SExtProp)];

                        DataRow row = scheme.Select($" {col_name_column_name} = '{col.ColumnName}' ").FirstOrDefault();

                        if (row == null)
                        {
                            Lib.Message.Make($"Error in database. Table [{dt.TableName}] doesn't have column [{col.ColumnName}].");
                            error = true;
                        }
                        else
                        {
                            if (!row[col_name_data_type].ToString().Equals(prop.data_type.ToString(), StringComparison.OrdinalIgnoreCase))
                            {
                                Lib.Message.Make($"Error in database. Table [{dt.TableName}] column [{col.ColumnName}] has different type is [{row[col_name_data_type]}]. Right data type is [{prop.data_type}]");
                                error = true;
                            }

                            if (prop.size > 0)
                            {
                                if (row[col_name_character_maximum_length] == System.DBNull.Value || (Int64)row[col_name_character_maximum_length] != prop.size)
                                {
                                    Lib.Message.Make($"Error in database. Table [{dt.TableName}] column [{col.ColumnName}] has different size. Right size is [{prop.size}]");
                                    error = true;
                                }
                            }


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string table_name = "unknown";
                if (dt != null)
                    table_name = dt.TableName;

                Lib.Message.Make($"Error check scheme table [{table_name}]", ex);
                error = true;
            }

            return !error;
        }

        private bool RowExists(DataRow dr)
        {
            bool result = false;

            try
            {

                string sql = String.Empty;
                DataColumn[] columns = dr.Table.Columns.Cast<DataColumn>()
                                            .Where(x => x.ExtendedProperties.ContainsKey(typeof(SExtProp)))
                                            .Where(x =>
                                                        {
                                                            SExtProp prop = (SExtProp)x.ExtendedProperties[typeof(SExtProp)];
                                                            return prop.primary_key;
                                                        }).ToArray();


                List<string> conditions = new List<string>();


                switch (type)
                {
                    case EType.MSSQLServer:
                        {

                            foreach (DataColumn col in columns)
                            {
                                conditions.Add($"[{col.ColumnName}] = ?");
                            }

                            sql = $@"SELECT [{string.Join("],[", columns.Select(x => x.ColumnName))}] FROM [{dr.Table.TableName}] WHERE {string.Join(" AND ", conditions)}";
                            break;
                        }
                    case EType.MySQL:
                    case EType.PostgreSQL:
                        {

                            foreach (DataColumn col in columns)
                            {
                                conditions.Add($"`{col.ColumnName}` = ?");
                            }

                            sql = $@"SELECT `{string.Join("`,`", columns.Select(x => x.ColumnName))}` FROM `{dr.Table.TableName}` WHERE {string.Join(" AND ", conditions)}";
                            break;

                        }
                }

                lock (command)
                {
                    command.Parameters.Clear();
                    command.CommandText = sql;

                    foreach (DataColumn col in columns)
                    {
                        SExtProp ext_prop = (SExtProp)col.ExtendedProperties[typeof(SExtProp)];
                        command.Parameters.Add("", ext_prop.data_type).Value = dr[col];
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        result = reader.HasRows;
                    }
                    //reader.Close();
                }

            }
            catch (Exception ex)
            {

                throw new Exception("Error to check row exists", ex);
            }

            return result;
        }

        private bool RowInsert(DataRow dr)
        {
            try
            {

                string sql = string.Empty;
                DataColumn[] columns = dr.Table.Columns.Cast<DataColumn>()
                                            .Where(x => x.ExtendedProperties.ContainsKey(typeof(SExtProp)))
                                            .Where(x =>
                                            {
                                                SExtProp prop = (SExtProp)x.ExtendedProperties[typeof(SExtProp)];
                                                return !prop.ignore;
                                            }).ToArray();


                char[] q = new char[columns.Length];
                for (int i = 0; i < q.Length; i++) { q[i] = '?'; }

                switch (type)
                {
                    case EType.MSSQLServer:
                        {
                            sql = $@"INSERT INTO [{dr.Table.TableName}] ([{string.Join("],[", columns.Select(x => x.ColumnName))}]) VALUES ({string.Join(" , ", q)})";
                            break;
                        }
                    case EType.MySQL:
                    case EType.PostgreSQL:
                        {
                            sql = $@"INSERT `{dr.Table.TableName}` (`{string.Join("`,`", columns.Select(x => x.ColumnName))}`) VALUES ({string.Join(" , ", q)})";
                            break;

                        }
                }

                lock (command)
                {

                    command.Parameters.Clear();
                    command.CommandText = sql;

                    foreach (DataColumn col in columns)
                    {
                        SExtProp ext_prop = (SExtProp)col.ExtendedProperties[typeof(SExtProp)];
                        command.Parameters.Add("", ext_prop.data_type).Value = dr[col];
                    }

                    command.ExecuteNonQuery();

                }


            }
            catch (Exception ex)
            {

                Lib.Message.Make("Error insert row", ex);
                return false;
            }

            return true;
        }

        private bool RowUpdate(DataRow dr)
        {

            try
            {

                string sql = string.Empty;

                DataColumn[] pk_columns = dr.Table.Columns.Cast<DataColumn>()
                                        .Where(x => x.ExtendedProperties.ContainsKey(typeof(SExtProp)))
                                        .Where(x =>
                                        {
                                            SExtProp prop = (SExtProp)x.ExtendedProperties[typeof(SExtProp)];
                                            return prop.primary_key;
                                        }).ToArray();

                DataColumn[] columns = dr.Table.Columns.Cast<DataColumn>()
                                        .Where(x => x.ExtendedProperties.ContainsKey(typeof(SExtProp)))
                                        .Where(x =>
                                        {
                                            SExtProp prop = (SExtProp)x.ExtendedProperties[typeof(SExtProp)];
                                            return !prop.primary_key && !prop.ignore;
                                        }).ToArray();

                List<string> conditions = new List<string>();
                List<string> sets = new List<string>();

                switch (type)
                {
                    case EType.MSSQLServer:
                        {

                            foreach (DataColumn col in columns)
                            {
                                sets.Add($"[{col.ColumnName}] = ?");
                            }

                            foreach (DataColumn col in pk_columns)
                            {
                                conditions.Add($"[{col.ColumnName}] = ?");
                            }

                            sql = $@"UPDATE [{dr.Table.TableName}] SET {string.Join(" , ", sets)} WHERE {string.Join(" AND ", conditions)}";
                            break;
                        }
                    case EType.MySQL:
                    case EType.PostgreSQL:
                        {
                            foreach (DataColumn col in columns)
                            {
                                sets.Add($"`{col.ColumnName}` = ?");
                            }

                            foreach (DataColumn col in pk_columns)
                            {
                                conditions.Add($"`{col.ColumnName}` = ?");
                            }

                            sql = $@"UPDATE `{dr.Table.TableName}` SET {string.Join(" , ", sets)} WHERE {string.Join(" AND ", conditions)}";
                            break;

                        }
                }

                lock (command)
                {
                    command.Parameters.Clear();
                    command.CommandText = sql;

                    foreach (DataColumn col in columns)
                    {
                        SExtProp ext_prop = (SExtProp)col.ExtendedProperties[typeof(SExtProp)];
                        command.Parameters.Add("", ext_prop.data_type).Value = dr[col];
                    }

                    foreach (DataColumn col in pk_columns)
                    {
                        SExtProp ext_prop = (SExtProp)col.ExtendedProperties[typeof(SExtProp)];
                        command.Parameters.Add("", ext_prop.data_type).Value = dr[col];
                    }

                    command.ExecuteNonQuery();
                }


            }
            catch (Exception ex)
            {
                Lib.Message.Make("Error update row", ex);
                return false;
            }

            return true;

        }
        */
        #endregion

    }
}
