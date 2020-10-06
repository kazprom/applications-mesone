using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OPC_DB_gate_server
{
    class RT_values
    {

        #region CONSTANTS

        public const string col_name_tags_id = "tags_id";
        public const string col_name_timestamp = "timestamp";
        public const string col_name_value_raw = "value_raw";
        public const string col_name_value_str = "value_str";
        public const string col_name_quality = "quality";

        #endregion

        #region VARIABLES



        #endregion


        #region PROPERTIES

        private DBTable source = new DBTable("rt_values");
        public DBTable Source { get { return source; } }

        #endregion


        public RT_values()
        {

            source.AddColumn(col_name_tags_id, typeof(int));
            source.AddColumn(col_name_timestamp, typeof(DateTime));
            source.AddColumn(col_name_value_raw, typeof(byte[]));
            source.AddColumn(col_name_value_str, typeof(string));
            source.AddColumn(col_name_quality, typeof(byte));

        }


        public void Put(OPC_DB_gate_Lib.TagData tag)
        {
            try
            {

                DataRow row = source.Table.Rows.Find(tag.id);

                if (row == null)
                {
                    row = source.Table.NewRow();
                    row[DBTable.col_name_id] = tag.id;
                    source.Table.Rows.Add(row);
                }

                row[col_name_tags_id] = tag.id;
                row[col_name_timestamp] = tag.timestamp;
                row[col_name_value_raw] = OPC_DB_gate_Lib.TagData.ObjToBin(tag.value);
                row[col_name_value_str] = tag.value.ToString();
                row[col_name_quality] = tag.id;
                /*
                Console.WriteLine(source.Table.TableName);
                for (int i = 0; i < source.Table.Columns.Count; i++)
                {
                    Console.Write($"{source.Table.Columns[i].ColumnName}|");
                }
                    Console.WriteLine();

                foreach (DataRow item in source.Table.Rows)
                {
                    for (int i = 0; i < item.ItemArray.Length; i++)
                    {
                        Console.Write($"{item.ItemArray[i]} ");
                    }
                        Console.WriteLine();
                }
                */
            }
            catch (Exception ex)
            {
                throw new Exception("Error put", ex);
            }
        }

    }
}
