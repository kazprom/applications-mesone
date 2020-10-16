using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace LibDBgate
{
    public class BaseSettingsTag : BaseSettingsNDE
    {
        public int clients_id;
        public int rate;
        public TagData.EDataType data_type;
        public bool rt_value_enabled;
        public bool history_enabled;


        public static void SettingFromDataRow(DataRow row, ref BaseSettingsTag obj)
        {
            BaseSettingsNDE r_obj = obj;
            BaseSettingsNDE.SettingFromDataRow(row, ref r_obj);

            obj.clients_id = (int)row[BaseTableTags.col_name_clients_id];
            obj.rate = (int)row[BaseTableTags.col_name_rate];
            obj.data_type = (TagData.EDataType)row[BaseTableTags.col_name_data_type];
            obj.history_enabled = (bool)row[BaseTableTags.col_name_history_enabled];
            obj.enabled = (bool)row[BaseTableTags.col_name_enabled];

        }

    }
}
