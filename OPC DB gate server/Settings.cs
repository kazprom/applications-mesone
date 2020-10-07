using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OPC_DB_gate_server
{
    public class Settings
    {

        #region CONSTANTS

        public const string col_name_id = "id";
        public const string col_name_key = "key";
        public const string col_name_value = "value";

        #endregion


        #region PROPERTIES

        private DataTable source = new DataTable("settings");
        public DataTable Source { get { return source; } }


        private Lib.Parameter<int> depth_history_hour = new Parameter<int>("DB DEPTH_HISTORY_HOUR");
        public Lib.Parameter<int> DEPTH_HISTORY_HOUR { get { return depth_history_hour; } }

        private Lib.Parameter<int> depth_log_day = new Parameter<int>("DB DEPTH_LOG_DAY");
        public Lib.Parameter<int> DEPTH_LOG_DAY { get { return depth_log_day; } }

        private Lib.Parameter<bool> rt_value = new Parameter<bool>("DB RT_VALUES");
        public Lib.Parameter<bool> RT_VALUES { get { return rt_value; } }

        private Lib.Parameter<bool> history_write = new Parameter<bool>("DB HISTORY_WRITE");
        public Lib.Parameter<bool> HISTORY_WRITE { get { return history_write; } }

        #endregion


        public Settings()
        {

            source.Columns.Add(col_name_id, typeof(int));
            source.Columns.Add(col_name_key, typeof(string)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp() { primary_key = true });
            source.Columns.Add(col_name_value, typeof(string));


            source.RowChanged += ValueHandler;
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
                    case "RT_VALUES":
                        {
                            bool result;
                            if (bool.TryParse(value, out result))
                                rt_value.Value = result;
                            break;
                        }
                    case "HISTORY_WRITE":
                        {
                            bool result;
                            if (bool.TryParse(value, out result))
                                history_write.Value = result;
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
