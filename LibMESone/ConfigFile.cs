using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LibMESone
{
    public class ConfigFile
    {

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region VARIABLES

        private Lib.XML file = new Lib.XML();
        private Timer timer;

        #endregion

        #region PROPERTIES

        private string path;
        public string Path { get { return path; } set { path = value; logger.Info($"Path to config file = {path}"); } }


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

                Path = $@"{Lib.Common.PathExeFolder}{Lib.Common.NameExeFile.Split('.')[0]}.xml";

                FileHandler(null);
                timer = new Timer(FileHandler, null, 0, 60000);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Constructor");
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

            }
            catch (Exception ex)
            {
               logger.Error(ex, "Error to handle config file");
            }

        }

        #endregion

    }
}
