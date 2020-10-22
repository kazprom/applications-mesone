using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;

namespace S7_DB_gate.Tables
{

    public class Ttags : LibDBgate.Tables.Ttags
    {

        public new class Row : LibDBgate.Tables.Ttags.Row
        {

            public S7.Net.DataType plc_data_type;
            public int datablock_no;
            public int datablock_offset;
            public S7.Net.VarType req_type;

            public static void DataRowToRow(DataRow row, ref Row r_row)
            {
                LibDBgate.Tables.Ttags.Row b_row = r_row;
                LibDBgate.Tables.Ttags.Row.DataRowToRow(row, ref b_row);

                r_row.plc_data_type = (S7.Net.DataType)Enum.Parse(typeof(S7.Net.DataType), (string)row[col_name_plc_data_type]);
                r_row.datablock_no = (int)row[col_name_datablock_no];
                r_row.datablock_offset = (int)row[col_name_datablock_offset];
                r_row.req_type = (S7.Net.VarType)Enum.Parse(typeof(S7.Net.VarType), (string)row[col_name_req_type]);

            }
        }

        #region CONSTANTS

        public const string col_name_plc_data_type = "plc_data_type";
        public Lib.Database.SExtProp prop_plc_data_type = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.VarChar,
            size = 10
        };

        public const string col_name_datablock_no = "datablock_no";
        public Lib.Database.SExtProp prop_datablock_no = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.Int
        };


        public const string col_name_datablock_offset = "datablock_offset";
        public Lib.Database.SExtProp prop_datablock_offset = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.Int
        };

        public const string col_name_req_type = "req_type";
        public Lib.Database.SExtProp prop_req_type = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.VarChar,
            size = 15
        };

        #endregion

        public Ttags()
        {
            try
            {
                source.TableName = "tags";

                source.Columns.Add(col_name_plc_data_type, typeof(string)).ExtendedProperties.Add(prop_plc_data_type.GetType(),prop_plc_data_type);
                source.Columns.Add(col_name_datablock_no, typeof(int)).ExtendedProperties.Add(prop_datablock_no.GetType(), prop_datablock_no);
                source.Columns.Add(col_name_datablock_offset, typeof(int)).ExtendedProperties.Add(prop_datablock_offset.GetType(), prop_datablock_offset);
                source.Columns.Add(col_name_req_type, typeof(string)).ExtendedProperties.Add(prop_req_type.GetType(), prop_req_type);

            }
            catch (Exception ex)
            {
                throw new Exception("Error constructor", ex);
            }
        }
    }
}
