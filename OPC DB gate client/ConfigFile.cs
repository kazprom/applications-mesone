using Lib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace OPC_DB_gate_client
{
    public class ConfigFile
    {

        #region VARIABLES

        private Lib.XML file = new XML();
        private Thread thread;
        private bool execution = true;

        #endregion

        #region PROPERTIES

        private string path;
        public string PATH { get { return path; } }

        private Lib.Parameter<int> server_port = new Lib.Parameter<int>("XML SERVER PORT");
        public Lib.Parameter<int> SERVER_PORT { get { return server_port; } }

        private Lib.Parameter<IPAddress> server_ip = new Lib.Parameter<IPAddress>("XML SERVER IP");
        public Lib.Parameter<IPAddress> SERVER_IP { get { return server_ip; } }

        private Lib.Parameter<int> depth_log_day = new Lib.Parameter<int>("XML DEPTH_LOG_DAY");
        public Lib.Parameter<int> DEPTH_LOG_DAY { get { return depth_log_day; } }


        #endregion


        #region CONSTRUCTOR

        public ConfigFile(string path)
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

                    Logger.WriteMessage("Error config file", ex);
                }

                Thread.Sleep(100);

            }
        }

        private void MainAction()
        {
            try
            {
                file.Path = path;

                int depth_log_day_result;

                if (int.TryParse(file.ReadValue("DEPTH_LOG_DAY", Lib.TextLogCleaner.default_depth_day.ToString()), out depth_log_day_result))
                {
                    depth_log_day.Value = depth_log_day_result;
                }

                IPAddress server_ip_result;

                if (IPAddress.TryParse(file.ReadValue("SERVER/IP", TCPconnection.default_ip_address.ToString()), out server_ip_result))
                {
                    server_ip.Value = server_ip_result;
                }

                int server_port_result;

                if (int.TryParse(file.ReadValue("SERVER/PORT", TCPconnection.default_port.ToString()), out server_port_result))
                {
                    server_port.Value = server_port_result;
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
