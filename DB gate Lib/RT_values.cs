using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DB_gate_Lib
{
    public class RT_values
    {

        #region CONSTANTS

        public const string col_name_id = "id";
        public const string col_name_tags_id = "tags_id";
        public const string col_name_timestamp = "timestamp";
        public const string col_name_value_raw = "value_raw";
        public const string col_name_value_str = "value_str";
        public const string col_name_quality = "quality";

        #endregion

        #region VARIABLES



        #endregion


        #region PROPERTIES

        private DataTable source = new DataTable("rt_values");
        public DataTable Source { get { return source; } }

        #endregion


        public RT_values()
        {

            source.Columns.Add(col_name_id, typeof(int)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp),
                                                                                new Lib.Database.SExtProp()
                                                                                {
                                                                                    data_type = System.Data.Odbc.OdbcType.BigInt,
                                                                                    ignore = true
                                                                                });
            source.Columns.Add(col_name_tags_id, typeof(int)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp),
                                                                                     new Lib.Database.SExtProp()
                                                                                     {
                                                                                         data_type = System.Data.Odbc.OdbcType.BigInt,
                                                                                         primary_key = true
                                                                                     });
            source.Columns.Add(col_name_timestamp, typeof(DateTime)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp),
                                                                                     new Lib.Database.SExtProp()
                                                                                     {
                                                                                         data_type = System.Data.Odbc.OdbcType.DateTime,
                                                                                     });
            source.Columns.Add(col_name_value_raw, typeof(byte[])).ExtendedProperties.Add(typeof(Lib.Database.SExtProp),
                                                                                     new Lib.Database.SExtProp()
                                                                                     {
                                                                                         data_type = System.Data.Odbc.OdbcType.Binary,
                                                                                     });
            source.Columns.Add(col_name_value_str, typeof(string)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp),
                                                                                     new Lib.Database.SExtProp()
                                                                                     {
                                                                                         data_type = System.Data.Odbc.OdbcType.VarChar,
                                                                                     });
            source.Columns.Add(col_name_quality, typeof(byte)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp),
                                                                                     new Lib.Database.SExtProp()
                                                                                     {
                                                                                         data_type = System.Data.Odbc.OdbcType.SmallInt,
                                                                                     });

        }


        public void Put(TagData tag)
        {
            try
            {
                DataRow row;

                lock (source)
                {
                    row = source.Select($"{col_name_id} = {tag.id}").FirstOrDefault();

                    if (row == null)
                    {
                        row = source.NewRow();
                        row[col_name_tags_id] = tag.id;
                        source.Rows.Add(row);
                    }
                }


                row[col_name_timestamp] = tag.timestamp;
                row[col_name_value_raw] = TagData.ObjToBin(tag.value);
                row[col_name_value_str] = tag.value.ToString();
                row[col_name_quality] = tag.quality;

            }
            catch (Exception ex)
            {
                throw new Exception("Error put", ex);
            }
        }

    }
}
