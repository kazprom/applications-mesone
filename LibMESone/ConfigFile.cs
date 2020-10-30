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
        public string Path { get { return path; } set { path = value; Lib.Message.Make($"Path to config file = {path}"); } }
        /*
        private Lib.Parameter<Lib.Database.EType> db_type = new Lib.Parameter<Lib.Database.EType>("FILE DB_TYPE");
        public Lib.Parameter<Lib.Database.EType> DB_TYPE { get { return db_type; } }

        private Lib.Parameter<string> connection_string = new Lib.Parameter<string>("FILE CONNECTION_STRING");
        public Lib.Parameter<string> CONNECTION_STRING { get { return connection_string; } }
        */

        /*
        private Lib.Parameter<int> depth_log_day = new Lib.Parameter<int>("FILE DEPTH_LOG_DAY");
        public Lib.Parameter<int> LOG_DEPTH_DAY { get { return depth_log_day; } }
        */


        private Lib.Parameter<string> db_driver = new Lib.Parameter<string>("CONFIG DB DRIVER");
        public Lib.Parameter<string> DB_DRIVER { get { return db_driver; } }

        private Lib.Parameter<string> db_host = new Lib.Parameter<string>("CONFIG DB HOST");
        public Lib.Parameter<string> DB_HOST { get { return db_host; } }

        private Lib.Parameter<int> db_port = new Lib.Parameter<int>("CONFIG DB PORT");
        public Lib.Parameter<int> DB_PORT { get { return db_port; } }

        private Lib.Parameter<string> db_charset = new Lib.Parameter<string>("CONFIG DB CHARSET");
        public Lib.Parameter<string> DB_CHARSET { get { return db_charset; } }

        private Lib.Parameter<string> db_base_name = new Lib.Parameter<string>("CONFIG DB BASE NAME");
        public Lib.Parameter<string> DB_BASE_NAME { get { return db_base_name; } }

        private Lib.Parameter<string> db_user = new Lib.Parameter<string>("CONFIG DB USER");
        public Lib.Parameter<string> DB_USER { get { return db_user; } }

        private Lib.Parameter<string> db_password = new Lib.Parameter<string>("CONFIG DB PASSWORD");
        public Lib.Parameter<string> DB_PASSWORD { get { return db_password; } }

        #endregion


        #region CONSTRUCTOR

        public ConfigFile()
        {
            try
            {

                Path = $@"{Lib.Global.PathExeFolder}{Lib.Global.NameExeFile.Split('.')[0]}.xml";

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

                db_driver.Value = file.ReadValue("DB/DRIVER", Lib.Database.default_driver);
                db_host.Value = file.ReadValue("DB/HOST", Lib.Database.default_host);
                
                int db_port_value;
                if (int.TryParse(file.ReadValue("DB/PORT", Lib.Database.default_port.ToString()), out db_port_value))
                    db_port.Value = db_port_value;

                db_charset.Value = file.ReadValue("DB/CHARSET", Lib.Database.default_charset);
                db_base_name.Value = file.ReadValue("DB/BASE_NAME", Lib.Database.default_base_name);
                db_user.Value = file.ReadValue("DB/USER", Lib.Database.default_user);
                db_password.Value = file.ReadValue("DB/PASSWORD", Lib.Database.default_password);


                /*

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
                */
            }
            catch (Exception ex)
            {
                Lib.Message.Make("Error to handle config file", ex);
            }

        }

        #endregion

    }
}
