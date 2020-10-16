using System;
using System.Data;
using System.Linq;

namespace Lib
{
    public class Application:BaseTable
    {

        #region CONSTANTS

        public const string col_name_key = "key";
        public const string col_name_value = "value";

        #endregion

        #region ENUMS

        public enum EKeys
        {
            APPINFO,
            CLOCK

        }

        #endregion

        #region PROPERTIES

        public DataTable Source { get { return source; } }

        #endregion

        public Application()
        {
            try
            {
                source.TableName = "application";

                Lib.Database.SExtProp prop = (Lib.Database.SExtProp)source.Columns[BaseTable.col_name_id].ExtendedProperties[typeof(Lib.Database.SExtProp)];
                prop.primary_key = false;
                prop.ignore = true;
                source.Columns[BaseTable.col_name_id].ExtendedProperties[typeof(Lib.Database.SExtProp)] = prop;
                source.Columns.Add(col_name_key, typeof(string)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp() {  data_type = System.Data.Odbc.OdbcType.VarChar,  primary_key = true });
                source.Columns.Add(col_name_value, typeof(string)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp() { data_type = System.Data.Odbc.OdbcType.VarChar});

            }
            catch (Exception ex)
            {
                throw new Exception("Error constructor", ex);
            }

        }


        public void Put(EKeys key, string value)
        {
            try
            {
                DataRow row;

                lock (source)
                {
                    row = source.Select($"{col_name_key} = '{key}'").FirstOrDefault();

                    if (row == null)
                    {
                        row = source.NewRow();
                        row[col_name_key] = key.ToString();
                        source.Rows.Add(row);
                    }
                }

                row[col_name_value] = value;
            }
            catch (Exception ex)
            {
                throw new Exception("Error put", ex);
            }
        }

    }
}
