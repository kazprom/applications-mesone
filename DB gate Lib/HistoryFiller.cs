using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace LibDBgate
{
    public class HistoryFiller
    {

        #region CONSTANTS

        public const char separator = '_';

        public const string table_prefix = "t";

        public const string col_name_tags_id = "tags_id";
        public const string col_name_timestamp = "timestamp";
        public const string col_name_value = "value";
        public const string col_name_quality = "quality";

        #endregion

        #region PROPERTIES

        private DataSet source = new DataSet("history");
        public DataSet Source { get { return source; } }

        #endregion

        public void Put(TagData tag)
        {
            try
            {
                lock (source)
                {
                    string table_name = Tables.Tt_.GetTableName(tag.timestamp);

                    DataTable table = source.Tables[table_name];
                    if (table == null)
                        source.Tables.Add(new Tables.Tt_(tag.timestamp).Source);

                    DataRow row = table.NewRow();

                    row[col_name_tags_id] = tag.id;
                    row[col_name_timestamp] = tag.timestamp;
                    row[col_name_value] = TagData.ObjToBin(tag.value);
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

    }
}
