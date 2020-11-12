using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LibMESone
{
    public class ConfigFile
    {

        #region CONSTANTS

#if DEBUG
        private const int period = 5000;
#else
        private const int period = 60000;
#endif


        #endregion

        #region VARIABLES

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private Lib.XML file = new Lib.XML();
        private Timer timer;

        #endregion

        #region EVENTS

        public delegate void ReadCompletedNotify(ConfigFile sender);  // delegate
        public event ReadCompletedNotify ReadCompleted; // event

        #endregion

        #region PROPERTIES

        public string Title { get; private set; }

        public string Path { get; private set; }

        public string DB_Driver { get; private set; }

        public string DB_Host { get; private set; }

        public uint DB_Port { get; private set; }

        public string DB_Charset { get; private set; }

        public string DB_BaseName { get; private set; }

        public string DB_User { get; private set; }

        public string DB_Password { get; private set; }

        #endregion

        #region CONSTRUCTOR

        public ConfigFile()
        {
            try
            {

                Title = $"Config file";

                Path = $@"{Lib.Common.PathExeFolder}{Lib.Common.NameExeFile.Split('.')[0]}.xml";
                logger.Info($"{Title}. Path = {Path}");


                timer = new Timer(FileHandler, null, 0, period);

            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Title}. Constructor");
            }

        }

        #endregion

        #region PRIVATES

        private void FileHandler(object state)
        {
            try
            {
                file.Path = Path;

                string result = "";

                result = file.ReadValue("DB/DRIVER", Lib.Database.default_driver);
                if (DB_Driver != result) { DB_Driver = result; logger.Info($"{Title}. DB DRIVER = {DB_Driver}"); }


                result = file.ReadValue("DB/HOST", Lib.Database.default_host);
                if (DB_Host != result) { DB_Host = result; logger.Info($"{Title}. DB HOST = {DB_Host}"); }

                uint port;
                if (uint.TryParse(file.ReadValue("DB/PORT", Lib.Database.default_port.ToString()), out port))
                {
                    if (DB_Port != port) { DB_Port = port; logger.Info($"{Title}. DB PORT = {DB_Port}"); }
                }

                result = file.ReadValue("DB/CHARSET", Lib.Database.default_charset);
                if (DB_Charset != result) { DB_Charset = result; logger.Info($"{Title}. DB CHARSET = {DB_Charset}"); }

                result = file.ReadValue("DB/BASE_NAME", Lib.Database.default_base_name);
                if (DB_BaseName != result) { DB_BaseName = result; logger.Info($"{Title}. DB BASE_NAME = {DB_Host}"); }

                result = file.ReadValue("DB/USER", Lib.Database.default_user);
                if (DB_User != result) { DB_User = result; logger.Info($"{Title}. DB USER = {DB_User}"); }

                result = file.ReadValue("DB/PASSWORD", Lib.Database.default_password);
                if (DB_Password != result) { DB_Password = result; logger.Info($"{Title}. DB PASSWORD = {DB_Password}"); }

                ReadCompleted?.Invoke(this);

            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Title}. File handler");
            }

        }

        #endregion

    }
}
