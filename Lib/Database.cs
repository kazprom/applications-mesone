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

        enum EType
        {
            Unknown = 0,
            MSSQLServer = 1,
            MySQL = 2,
            PostgreSQL = 3
        }

        #endregion

        #region PROPERTIES

        private EType type = EType.MySQL;
        public string Type { get { return type.ToString(); } set { Enum.TryParse(value, out type); } }

        private string connection_string = "Driver ={mySQL ODBC 8.0 ANSI Driver}; Server=myServerAddress;Option=131072;Stmt=;Database=myDataBase;User=myUsername;Password=myPassword;";
        public string ConnectionString { get { return connection_string; } set { connection_string = value; } }

        #endregion

        #region VARIABLE

        private OdbcConnection connection = new OdbcConnection();
        private OdbcCommand command = new OdbcCommand();

        #endregion


        #region PUBLICS

        public void Read(ref DataSet ds)
        {
            try
            {

                TestConnection();

                string sql = string.Empty;


                foreach (DataTable table in ds.Tables)
                {
                    string[] columns_names = table.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();

                    switch (type)
                    {
                        case EType.MSSQLServer:
                            {
                                sql = $"SELECT [{string.Join("],[", columns_names)}] FROM [{table.TableName}]";
                                break;
                            }
                        case EType.MySQL:
                        case EType.PostgreSQL:
                            {
                                sql = $"SELECT `{string.Join("`,`", columns_names)}` FROM `{table.TableName}`";
                                break;
                            }
                    }

                    

                    if (table.PrimaryKey.Length > 0)
                    {
                        DataTable result = table.Clone();
                        lock (connection)
                        {
                            OdbcDataAdapter adapter = new OdbcDataAdapter(sql, connection);
                            adapter.Fill(result);
                        }

                        DataTable unnecessary = table.Copy();
                        foreach (DataRow row in result.Rows)
                        {
                            List<object> pk = new List<object>();
                            foreach (DataColumn dc in result.PrimaryKey)
                            {
                                pk.Add(row.Field<object>(dc));
                            }

                            DataRow fr = table.Rows.Find(pk.ToArray());
                            if (fr == null)
                            {
                                table.Rows.Add(row.ItemArray);
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
                                pk.Add(row.Field<object>(dc));
                            }
                            DataRow dr = table.Rows.Find(pk.ToArray());
                            dr.Delete();
                        }
                    }
                    else
                    {
                        lock (connection)
                        {
                            OdbcDataAdapter adapter = new OdbcDataAdapter(sql, connection);
                            adapter.Fill(table);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error read", ex);
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
