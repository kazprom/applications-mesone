using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Lib
{
    public class DBLogger
    {


        #region CONSTANTS

        public const char separator = '_';

        public const string table_prefix = "l";

        public const string col_name_timestamp = "timestamp";
        public const string col_name_message = "message";

        #endregion

        #region PROPERTIES

        private Database database;
        public Database Database { get { return database; } }

        #endregion


        public DBLogger(Database database)
        {

            this.database = database;

            Message.FullMsgMaked += Write;

        }


        private void Write(string str)
        {
            try
            {

                if (database != null)
                {
                    DataTable table = new DataTable(GetTableName(DateTime.Now));

                    table.Columns.Add(BaseTable.col_name_id, typeof(int)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp),
                                                                                           new Lib.Database.SExtProp()
                                                                                           {
                                                                                               data_type = System.Data.Odbc.OdbcType.BigInt,
                                                                                               primary_key = true,
                                                                                               auto_increment = true,
                                                                                               not_null = true,
                                                                                               ignore = true
                                                                                           });

                    table.Columns.Add(col_name_timestamp, typeof(DateTime)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp),
                                                                                                              new Lib.Database.SExtProp()
                                                                                                              {
                                                                                                                  data_type = System.Data.Odbc.OdbcType.DateTime,
                                                                                                                  size = 3
                                                                                                              });

                    table.Columns.Add(col_name_message, typeof(string)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp),
                                                                                                              new Lib.Database.SExtProp()
                                                                                                              {
                                                                                                                  data_type = System.Data.Odbc.OdbcType.Text
                                                                                                              });

                    DataRow row = table.NewRow();
                    row[col_name_timestamp] = DateTime.Now;
                    row[col_name_message] = str;
                    table.Rows.Add(row);

                    database.Write(table, true, false);
                }

            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error write log to database {ex.Message}");
            }

        }


        public static string GetTableName(DateTime timestamp)
        {
            return table_prefix + separator + timestamp.ToString("yyyy" + separator + "MM" + separator + "dd");
        }

    }
}
