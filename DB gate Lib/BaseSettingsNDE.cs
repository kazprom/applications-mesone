using System;
using System.Data;

namespace LibDBgate
{
    public class BaseSettingsNDE : Lib.BaseSettings
    {
        public string name;
        //public string description;
        public bool enabled;

        public static void SettingFromDataRow(DataRow row, ref BaseSettingsNDE obj)
        {

            Lib.BaseSettings r_obj = obj;
            Lib.BaseSettings.SettingFromDataRow(row, ref r_obj);

            obj.name = (string)row[BaseTableNDE.col_name_name];
            //obj.description = (string)row[BaseTableNDE.col_name_name];
            obj.enabled = (bool)row[BaseTableNDE.col_name_enabled];
        
        }
    }
}
