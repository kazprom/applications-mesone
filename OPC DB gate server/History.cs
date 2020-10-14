using Lib;
using LibOPCDBgate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace OPC_DB_gate_server
{
    class History
    {

        #region CONSTANTS

        public const char separator = '_';

        public const string table_prefix = "t";

        public const string col_name_id = "id";
        public const string col_name_tags_id = "tags_id";
        public const string col_name_timestamp = "timestamp";
        public const string col_name_value = "value";
        public const string col_name_quality = "quality";

        #endregion



        #region PROPERTIES

        private DataSet source = new DataSet("history");
        public DataSet Source { get { return source; } }

        #endregion



        public void Put(LibDBgate.TagData tag)
        {
            try
            {
                lock (source)
                {
                    string table_name = GetTableName(tag.timestamp);

                    DataTable table = source.Tables[table_name];
                    if (table == null)
                    {
                        table = new DataTable(table_name);
                        table.Columns.Add(col_name_id, typeof(int)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp),
                                                                                           new Lib.Database.SExtProp()
                                                                                           {
                                                                                               data_type = System.Data.Odbc.OdbcType.BigInt,
                                                                                               primary_key = true,
                                                                                               auto_increment = true,
                                                                                               not_null = true
                                                                                           });
                        table.Columns.Add(col_name_tags_id, typeof(int)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp),
                                                                                                new Lib.Database.SExtProp()
                                                                                                {
                                                                                                    data_type = System.Data.Odbc.OdbcType.BigInt,
                                                                                                });
                        table.Columns.Add(col_name_timestamp, typeof(DateTime)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp),
                                                                                                              new Lib.Database.SExtProp()
                                                                                                              {
                                                                                                                  data_type = System.Data.Odbc.OdbcType.DateTime,
                                                                                                                  size = 3
                                                                                                              });
                        table.Columns.Add(col_name_value, typeof(byte[])).ExtendedProperties.Add(typeof(Lib.Database.SExtProp),
                                                                                                 new Lib.Database.SExtProp()
                                                                                                 {
                                                                                                     data_type = System.Data.Odbc.OdbcType.Binary,
                                                                                                     size = 8
                                                                                                 });
                        table.Columns.Add(col_name_quality, typeof(byte)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp),
                                                                                                 new Lib.Database.SExtProp()
                                                                                                 {
                                                                                                     data_type = System.Data.Odbc.OdbcType.SmallInt,
                                                                                                 }); ;
                        source.Tables.Add(table);
                    }

                    DataRow row = table.NewRow();

                    row[col_name_tags_id] = tag.id;
                    row[col_name_timestamp] = tag.timestamp;
                    row[col_name_value] = LibDBgate.TagData.ObjToBin(tag.value);
                    row[col_name_quality] = tag.quality;

                    table.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error put", ex);
            }
        }

        public void Clear()
        {
            try
            {
                lock (source)
                {
                    source.Reset();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error clear", ex);
            }
        }

        public static string GetTableName(DateTime timestamp)
        {
            return table_prefix + separator + timestamp.ToString("yyyy" + separator + "MM" + separator + "dd" + separator + "HH");
        }

    }
}
