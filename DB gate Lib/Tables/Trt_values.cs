using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LibDBgate
{
    public class Trt_values:LibMESone.Tables.BaseID
    {
        /*
        #region CONSTANTS

        public const string col_name_tags_id = "tags_id";
        public Lib.Database.SExtProp prop_tags_id = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.BigInt,
            primary_key = true
        };

        public const string col_name_timestamp = "timestamp";
        public Lib.Database.SExtProp prop_timestamp = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.Timestamp,
            size = 3
        };

        public const string col_name_value_raw = "value_raw";
        public Lib.Database.SExtProp prop_value_raw = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.Binary,
            size = 8
        };

        public const string col_name_value_str = "value_str";
        public Lib.Database.SExtProp prop_value_str = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.VarChar,
            size = 255
        };

        public const string col_name_quality = "quality";
        public Lib.Database.SExtProp prop_quality = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.SmallInt
        };

        #endregion

        public Trt_values()
        {

            source.TableName = "rt_values";
            
            Lib.Database.SExtProp prop = (Lib.Database.SExtProp)source.Columns[LibMESone.Tables.Tbase.col_name_id].ExtendedProperties[typeof(Lib.Database.SExtProp)];
            prop.ignore = true;
            prop.primary_key = false;

            source.Columns.Add(col_name_tags_id, typeof(int)).ExtendedProperties.Add(prop_tags_id.GetType(), prop_tags_id);
            source.Columns.Add(col_name_timestamp, typeof(DateTime)).ExtendedProperties.Add(prop_timestamp.GetType(), prop_timestamp);
            source.Columns.Add(col_name_value_raw, typeof(byte[])).ExtendedProperties.Add(prop_value_raw.GetType(), prop_value_raw);
            source.Columns.Add(col_name_value_str, typeof(string)).ExtendedProperties.Add(prop_value_str.GetType(), prop_value_str);
            source.Columns.Add(col_name_quality, typeof(byte)).ExtendedProperties.Add(prop_quality.GetType(), prop_quality);

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
        */
    }
}
