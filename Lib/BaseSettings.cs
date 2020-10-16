using System.Data;

namespace Lib
{
    public class BaseSettings
    {
        public int id;


        public static void SettingFromDataRow(DataRow row, ref BaseSettings obj)
        {
            obj.id = (int)row[BaseTable.col_name_id];
        }

    }
}
