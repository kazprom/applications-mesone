using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;

namespace Lib
{
    public class Database
    {

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

        private OdbcConnection connection = new OdbcConnection() ;
        private OdbcCommand command = new OdbcCommand() ;

        #endregion


        #region PUBLICS


        public void Read(DataSet ds)
        {
            try
            {
                foreach (DataTable table in ds.Tables)
                {
                    Read(table);
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

            }
            catch (Exception ex)
            {
                throw new Exception("Error write", ex);
            }
        }

        public void Write(DataTable dt, bool create_table, bool rewrite_row)
        {
            try
            {
                if (dt != null)
                {
                    TestConnection();



                    foreach (DataRow row in dt.Rows)
                    {

                        bool row_exist = false;

                        if (rewrite_row)
                        {

                            string sql = string.Empty;
                            string[] column_names = dt.PrimaryKey.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
                            char[] q = new char[column_names.Length];
                            for (int i = 0; i < q.Length; i++) { q[i] = '?'; }

                            string[] conditions = new string[column_names.Length];


                            switch (type)
                            {
                                case EType.MSSQLServer:
                                    {
                                        for (int i = 0; i < column_names.Length; i++)
                                        {
                                            conditions[i] = $"[{column_names[i]}] = {q[i]}";
                                        }

                                        sql = $@"SELECT [{string.Join("],[", column_names)}] FROM [{dt.TableName}] WHERE {string.Join(" AND ", conditions)}";
                                        break;
                                    }
                                case EType.MySQL:
                                case EType.PostgreSQL:
                                    {

                                        for (int i = 0; i < column_names.Length; i++)
                                        {
                                            conditions[i] = $"`{column_names[i]}` = {q[i]}";
                                        }

                                        sql = $@"SELECT `{string.Join("`,`", column_names)}` FROM `{dt.TableName}` WHERE {string.Join(" AND ", conditions)}";
                                        break;

                                    }
                            }

                            command.Parameters.Clear();
                            command.CommandText = sql;

                            foreach (string col in column_names)
                            {
                                command.Parameters.Add("", ConvertType(row[col])).Value = row[col];
                            }

                            var reader = command.ExecuteReader();
                            row_exist = reader.HasRows;
                            reader.Close();
                        }


                        if (!rewrite_row || !row_exist)
                        {
                            string sql = string.Empty;
                            string[] column_names = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
                            char[] q = new char[column_names.Length];
                            for (int i = 0; i < q.Length; i++) { q[i] = '?'; }

                            switch (type)
                            {
                                case EType.MSSQLServer:
                                    {
                                        sql = $@"INSERT INTO [{dt.TableName}] ([{string.Join("],[", column_names)}]) VALUES ({string.Join(" , ", q)})";
                                        break;
                                    }
                                case EType.MySQL:
                                case EType.PostgreSQL:
                                    {
                                        sql = $@"INSERT `{dt.TableName}` (`{string.Join("`,`", column_names)}`) VALUES ({string.Join(" , ", q)})";
                                        break;

                                    }
                            }

                            command.Parameters.Clear();
                            command.CommandText = sql;

                            foreach (string col in column_names)
                            {
                                command.Parameters.Add("", ConvertType(row[col])).Value = row[col];
                            }

                            command.ExecuteNonQuery();

                        }

                        if (rewrite_row)
                        {
                            string sql = string.Empty;
                            string[] pk_column_names = dt.PrimaryKey.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
                            string[] column_names = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
                            char[] pk_q = new char[pk_column_names.Length];
                            char[] q = new char[column_names.Length];
                            string[] conditions = new string[pk_column_names.Length];
                            string[] sets = new string[column_names.Length];

                            for (int i = 0; i < pk_q.Length; i++) { pk_q[i] = '?'; }
                            for (int i = 0; i < q.Length; i++) { q[i] = '?'; }


                            switch (type)
                            {
                                case EType.MSSQLServer:
                                    {

                                        for (int i = 0; i < sets.Length; i++)
                                        {
                                            sets[i] = $"[{column_names[i]}] = {q[i]}";
                                        }

                                        for (int i = 0; i < conditions.Length; i++)
                                        {
                                            conditions[i] = $"[{pk_column_names[i]}] = {pk_q[i]}";
                                        }

                                        sql = $@"UPDATE [{dt.TableName}] SET {string.Join(" , ", sets)} WHERE {string.Join(" AND ", conditions)}";
                                        break;
                                    }
                                case EType.MySQL:
                                case EType.PostgreSQL:
                                    {
                                        for (int i = 0; i < sets.Length; i++)
                                        {
                                            sets[i] = $"`{column_names[i]}` = {q[i]}";
                                        }

                                        for (int i = 0; i < conditions.Length; i++)
                                        {
                                            conditions[i] = $"`{pk_column_names[i]}` = {pk_q[i]}";
                                        }

                                        sql = $@"UPDATE `{dt.TableName}` SET {string.Join(" , ", sets)} WHERE {string.Join(" AND ", conditions)}";
                                        break;

                                    }
                            }

                            command.Parameters.Clear();
                            command.CommandText = sql;

                            foreach (string col in column_names)
                            {
                                command.Parameters.Add("", ConvertType(row[col])).Value = row[col];
                            }

                            foreach (string col in pk_column_names)
                            {
                                command.Parameters.Add("", ConvertType(row[col])).Value = row[col];
                            }

                            command.ExecuteNonQuery();

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error write data table", ex);
            }

        }

        public void Sync(DataSet ds)
        {

        }

        public void AddTables(DataSet ds)
        {
            try
            {
                TestConnection();

            }
            catch (Exception ex)
            {
                throw new Exception("Error add table", ex);
            }

        }

        public void DeleteTables(string[] names)
        {
            try
            {
                TestConnection();

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
                        command.CommandText = "select 1";
                        command.Parameters.Clear();
                        command.ExecuteNonQuery();
                    }
            }
            catch (Exception ex)
            {
                throw new Exception("Error connection", ex);

            }

        }

        private static OdbcType ConvertType(object obj)
        {
            if (obj is bool)
            {
                return OdbcType.TinyInt;
            }
            else if (obj is byte[])
            {
                return OdbcType.Binary;
            }
            else if (obj is int)
            {
                return OdbcType.Int;
            }
            else if (obj is DateTime)
            {
                return OdbcType.DateTime;
            }
            else
            {
                return OdbcType.Text;
            }
        }


        #endregion

        #region CLASEES

        class RowsComparer : IEqualityComparer<DataRow>
        {
            public bool Equals(DataRow x, DataRow y)
            {
                int id1 = (int)x[0];
                int id2 = (int)y[0];
                return id1 == id2;
            }

            public int GetHashCode(DataRow obj)
            {
                int hash = 0;
                foreach (var item in obj.ItemArray)
                {
                    hash += item.GetHashCode();
                }
                return hash;
            }
        }

        #endregion

    }
}
