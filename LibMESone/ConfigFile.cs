using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LibMESone
{
    public class ConfigFile
    {


        #region VARIABLES

        private Lib.XML file = new Lib.XML();
        private Timer timer;

        #endregion

        #region PROPERTIES

        private string path;
        public string Path { get { return path; }  set { path = value; Lib.Message.Make($"Path to config file = {path}"); } }

        private Lib.Parameter<Lib.Database.EType> db_type = new Lib.Parameter<Lib.Database.EType>("FILE DB_TYPE");
        public Lib.Parameter<Lib.Database.EType> DB_TYPE { get { return db_type; } }

        private Lib.Parameter<string> connection_string = new Lib.Parameter<string>("FILE CONNECTION_STRING");
        public Lib.Parameter<string> CONNECTION_STRING { get { return connection_string; } }

        private Lib.Parameter<int> depth_log_day = new Lib.Parameter<int>("FILE DEPTH_LOG_DAY");
        public Lib.Parameter<int> DEPTH_LOG_DAY { get { return depth_log_day; } }


        #endregion


        #region CONSTRUCTOR

        public ConfigFile()
        {
            try
            {

                Path = Lib.Global.NameExeFile.Split('.')[0] + ".xml";

                FileHandler(null);
                timer = new Timer(FileHandler, null, 0, 60000);

            }
            catch (Exception ex)
            {
                throw new Exception("Error constructor", ex);
            }

        }



        #endregion



        #region PRIVATES

      

        private void FileHandler(object state)
        {
            try
            {
                file.Path = path;

                Lib.Database.EType db_type_result;

                if (Enum.TryParse<Lib.Database.EType>(file.ReadValue("DB_TYPE", Lib.Database.default_type.ToString()), out db_type_result))
                {
                    db_type.Value = db_type_result;
                }

                connection_string.Value = file.ReadValue("CONNECTION_STRING", Lib.Database.default_connection_string);

                int depth_log_day_result;

                if (int.TryParse(file.ReadValue("DEPTH_LOG_DAY", Loggers.TextLogCleaner.default_depth_day.ToString()), out depth_log_day_result))
                {
                    depth_log_day.Value = depth_log_day_result;
                }
            }
            catch (Exception ex)
            {
                Lib.Message.Make("Error to handle config file", ex);
            }

        }

        #endregion

    }
}
