using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;

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

        public enum EType
        {
            Unknown = 0,
            MSSQLServer = 1,
            MySQL = 2,
            PostgreSQL = 3
        }

        #endregion

        #region PROPERTIES

        public const EType default_type = EType.MySQL;
        private EType type = default_type;
        public EType Type { get { return type; } set { type = value; } }

        public const string default_connection_string = "Driver ={mySQL ODBC 8.0 ANSI Driver}; Server=myServerAddress;Option=131072;Stmt=;Database=myDataBase;User=myUsername;Password=myPassword;";
        private string connection_string = default_connection_string;
        public string ConnectionString { get { return connection_string; } set { connection_string = value; } }

        #endregion

        #region VARIABLE

        private OdbcConnection connection = new OdbcConnection();
        private OdbcCommand command = new OdbcCommand();

        #endregion

        #region PUBLICS


        public void Read(DataSet ds)
        {
            try
            {
                lock (ds)
                {
                    foreach (DataTable table in ds.Tables)
                    {
                        Read(table);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error read data set", ex);
            }
        }
        public void Read(DataTable dt)
        {
            try
            {

                TestConnection();

                string sql = string.Empty;

                lock (dt)
                {
                    string[] columns_names = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();

                    switch (type)
                    {
                        case EType.MSSQLServer:
                            {
                                sql = $"SELECT [{string.Join("],[", columns_names)}] FROM [{dt.TableName}]";
                                break;
                            }
                        case EType.MySQL:
                        case EType.PostgreSQL:
                            {
                                sql = $"SELECT `{string.Join("`,`", columns_names)}` FROM `{dt.TableName}`";
                                break;
                            }
                    }



                    if (dt.PrimaryKey.Length > 0)
                    {
                        DataTable result = dt.Clone();
                        lock (connection)
                        {
                            OdbcDataAdapter adapter = new OdbcDataAdapter(sql, connection);
                            adapter.Fill(result);
                        }

                        DataTable unnecessary = dt.Copy();
                        foreach (DataRow row in result.Rows)
                        {
                            List<object> pk = new List<object>();
                            foreach (DataColumn dc in result.PrimaryKey)
                            {
                                //pk.Add(row.Field<object>(dc));
                                pk.Add(row.ItemArray[dc.Ordinal]);
                            }

                            DataRow fr = dt.Rows.Find(pk.ToArray());
                            if (fr == null)
                            {
                                dt.Rows.Add(row.ItemArray);
                            }
                            else
                            {
                                fr.ItemArray = row.ItemArray;
                                unnecessary.Rows.Remove(unnecessary.Rows.Find(pk.ToArray()));
                            }
                        }
                        foreach (DataRow row in unnecessary.Rows)
                        {
                            List<object> pk = new List<object>();
                            foreach (DataColumn dc in unnecessary.PrimaryKey)
                            {
                                //pk.Add(row.Field<object>(dc));
                                pk.Add(row.ItemArray[dc.Ordinal]);
                            }
                            DataRow dr = dt.Rows.Find(pk.ToArray());
                            dr.Delete();
                        }
                    }
                    else
                    {
                        lock (connection)
                        {
                            OdbcDataAdapter adapter = new OdbcDataAdapter(sql, connection);
                            adapter.Fill(dt);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error read data table", ex);
            }
        }


        public void Write(DataSet ds)
        {
            try
            {
                TestConnection();

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
                throw new Exception("Error write data set", ex);
            }
        }

        public void Write(DataTable dt, bool create_table, bool rewrite_row)
        {
            try
            {
                if (dt != null)
                {

                    TestConnection();

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
                throw new Exception("Error write data table", ex);
            }

        }



        public string[] GetListTables(string condition = "")
        {
            try
            {
                TestConnection();

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
                    OdbcDataAdapter adapter = new OdbcDataAdapter(sql, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt.Rows.Cast<DataRow>().Select(x => x["TABLE_NAME"].ToString()).ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error get list tables", ex);
            }
        }
        public void DeleteTables(string[] table_names)
        {
            try
            {
                TestConnection();

                foreach (string table_name in table_names)
                {
                    DeleteTable(table_name);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error delete tables", ex);
            }
        }

        public void DeleteTable(string table_name)
        {

            try
            {
                TestConnection();

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
                throw new Exception("Error delete table", ex);
            }

        }

        #endregion

        #region PRIVATES

        private void TestConnection()
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
                throw new Exception("Error connection", ex);

            }

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
                    var reader = command.ExecuteReader();
                    result = reader.HasRows;
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error to check table exists", ex);
            }

            return result;
        }

        private void TableAdd(DataTable dt)
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

                command.Parameters.Clear();
                command.CommandText = sql;
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw new Exception("Error add table", ex);
            }
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

                    var reader = command.ExecuteReader();
                    result = reader.HasRows;
                    reader.Close();
                }

            }
            catch (Exception ex)
            {

                throw new Exception("Error to check row exists", ex);
            }

            return result;
        }

        private void RowInsert(DataRow dr)
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

                command.Parameters.Clear();
                command.CommandText = sql;

                foreach (DataColumn col in columns)
                {
                    SExtProp ext_prop = (SExtProp)col.ExtendedProperties[typeof(SExtProp)];
                    command.Parameters.Add("", ext_prop.data_type).Value = dr[col];
                }

                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {

                throw new Exception("Error insert row", ex);
            }
        }

        private void RowUpdate(DataRow dr)
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
            catch (Exception ex)
            {

                throw new Exception("Error update row", ex);
            }

        }

        #endregion

    }
}
