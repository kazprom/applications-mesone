using Lib;
using System;
using System.Data;

namespace S7_DB_gate
{
    public class Settings: Lib.BaseTable
    {

        #region CONSTANTS

        public const string col_name_key = "key";
        public const string col_name_value = "value";

        #endregion


        #region PROPERTIES

        public DataTable Source { get { return source; } }


        private Lib.Parameter<int> depth_history_hour = new Parameter<int>("DB DEPTH_HISTORY_HOUR");
        public Lib.Parameter<int> DEPTH_HISTORY_HOUR { get { return depth_history_hour; } }

        private Lib.Parameter<int> depth_log_day = new Parameter<int>("DB DEPTH_LOG_DAY");
        public Lib.Parameter<int> DEPTH_LOG_DAY { get { return depth_log_day; } }

        #endregion


        public Settings()
        {
            try
            {

                source.TableName = "settings";

                source.Columns.Add(col_name_key, typeof(string)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
                {
                    data_type = System.Data.Odbc.OdbcType.VarChar,
                    size = 50,
                    primary_key = true
                });
                source.Columns.Add(col_name_value, typeof(string)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
                {
                    data_type = System.Data.Odbc.OdbcType.VarChar,
                    size = 255,
                });


                source.RowChanged += ValueHandler;

            }
            catch (Exception ex)
            {

                throw new Exception("Error constructor", ex);
            }


        }

        #region PRIVATES
        private void ValueHandler(object sender, System.Data.DataRowChangeEventArgs e)
        {
            try
            {

                string key = (string)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(col_name_key)];
                string value = (string)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(col_name_value)];

                switch (key.ToUpper())
                {
                    case "DEPTH_HISTORY_HOUR":
                        {
                            int result;
                            if (int.TryParse(value, out result))
                                depth_history_hour.Value = result;
                            break;
                        }
                    case "DEPTH_LOG_DAY":
                        {
                            int result;
                            if (int.TryParse(value, out result))
                                depth_log_day.Value = result;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error handling settings value", ex);
            }



        }

        #endregion

    }
}