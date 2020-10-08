using Lib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace OPC_DB_gate_server
{
    public class HandlerConfigFile
    {

        #region VARIABLES

        private Lib.XML file = new XML();
        private Thread thread;
        private bool execution = true;

        #endregion

        #region PROPERTIES

        private string path;
        public string PATH { get { return path; } }

        private Lib.Parameter<Lib.Database.EType> db_type = new Lib.Parameter<Lib.Database.EType>("XML DB_TYPE");
        public Lib.Parameter<Lib.Database.EType> DB_TYPE { get { return db_type; } }

        private Lib.Parameter<string> connection_string = new Lib.Parameter<string>("XML CONNECTION_STRING");
        public Lib.Parameter<string> CONNECTION_STRING { get { return connection_string; } }

        private Lib.Parameter<int> depth_log_day = new Lib.Parameter<int>("XML DEPTH_LOG_DAY");
        public Lib.Parameter<int> DEPTH_LOG_DAY { get { return depth_log_day; } }


        #endregion


        #region CONSTRUCTOR

        public HandlerConfigFile(string path)
        {
            this.path = path;

            MainAction();
            thread = new Thread(new ThreadStart(Handler)) { IsBackground = true, Name = "ConfigFile" };
            thread.Start();

        }



        #endregion



        #region PRIVATES

        private void Handler()
        {


            while (execution)
            {
                try
                {

#if DEBUG
                    if (DateTime.Now.Second % 5 == 0)
#else
                    if(DateTime.Now.Second == 0)
#endif
                    {

                        MainAction();
                        Thread.Sleep(1000);

                    }


                }
                catch (Exception ex)
                {

                    Lib.Message.Make("Error config file", ex);
                }

                Thread.Sleep(100);

            }
        }

        private void MainAction()
        {
            try
            {
                file.Path = path;

                object db_type_result;

                if (Enum.TryParse(typeof(Lib.Database.EType), file.ReadValue("DB_TYPE", Lib.Database.default_type.ToString()), out db_type_result))
                {
                    db_type.Value = (Lib.Database.EType)db_type_result;
                }

                connection_string.Value = file.ReadValue("CONNECTION_STRING", Lib.Database.default_connection_string);

                int depth_log_day_result;

                if (int.TryParse(file.ReadValue("DEPTH_LOG_DAY", Lib.TextLogCleaner.default_depth_day.ToString()), out depth_log_day_result))
                {
                    depth_log_day.Value = depth_log_day_result;
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error action", ex);
            }
            
        }

        #endregion

    }
}
