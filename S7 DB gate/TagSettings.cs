using System;
using System.Data;

namespace S7_DB_gate
{
    public class TagSettings: LibDBgate.BaseSettingsTag
    {

        public S7.Net.DataType plc_data_type;
        public int db_no;
        public int db_offset;
        public S7.Net.VarType req_type;


        public static void SettingFromDataRow(DataRow row, ref TagSettings obj)
        {
            LibDBgate.BaseSettingsTag r_obj = obj;
            LibDBgate.BaseSettingsTag.SettingFromDataRow(row, ref r_obj);

            obj.plc_data_type = (S7.Net.DataType)Enum.Parse(typeof(S7.Net.DataType),(string)row[Tags.col_name_plc_data_type]);
            obj.db_no = (int)row[Tags.col_name_db_no];
            obj.db_offset = (int)row[Tags.col_name_db_offset];
            obj.req_type = (S7.Net.VarType)Enum.Parse(typeof(S7.Net.VarType),(string)row[Tags.col_name_req_type]);

        }

    }





}
