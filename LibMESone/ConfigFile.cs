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

        public uint LOG_DepthDay { get; private set; }

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

                string str_result = "";
                uint uint_result;

                str_result = file.ReadValue("DB/DRIVER", Lib.Database.default_driver);
                if (DB_Driver != str_result) { DB_Driver = str_result; logger.Info($"{Title}. DB DRIVER = {DB_Driver}"); }


                str_result = file.ReadValue("DB/HOST", Lib.Database.default_host);
                if (DB_Host != str_result) { DB_Host = str_result; logger.Info($"{Title}. DB HOST = {DB_Host}"); }

                if (uint.TryParse(file.ReadValue("DB/PORT", Lib.Database.default_port.ToString()), out uint_result))
                {
                    if (DB_Port != uint_result) { DB_Port = uint_result; logger.Info($"{Title}. DB PORT = {DB_Port}"); }
                }

                str_result = file.ReadValue("DB/CHARSET", Lib.Database.default_charset);
                if (DB_Charset != str_result) { DB_Charset = str_result; logger.Info($"{Title}. DB CHARSET = {DB_Charset}"); }

                str_result = file.ReadValue("DB/BASE_NAME", Lib.Database.default_base_name);
                if (DB_BaseName != str_result) { DB_BaseName = str_result; logger.Info($"{Title}. DB BASE_NAME = {DB_Host}"); }

                str_result = file.ReadValue("DB/USER", Lib.Database.default_user);
                if (DB_User != str_result) { DB_User = str_result; logger.Info($"{Title}. DB USER = {DB_User}"); }

                str_result = file.ReadValue("DB/PASSWORD", Lib.Database.default_password);
                if (DB_Password != str_result) { DB_Password = str_result; logger.Info($"{Title}. DB PASSWORD = {DB_Password}"); }

                if (uint.TryParse(file.ReadValue("LOG/DEPTH_DAY", Lib.Common.default_log_depth_day.ToString()), out uint_result))
                {
                    if (LOG_DepthDay != uint_result) { LOG_DepthDay = uint_result; logger.Info($"{Title}. LOG DEPTH DAY = {LOG_DepthDay}"); }
                }

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
