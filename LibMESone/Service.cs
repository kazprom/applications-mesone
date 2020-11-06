using LibMESone.Structs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace LibMESone
{
    public class Service : IDisposable
    {


        #region VARIABLES

        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private Core parent;
        private string name;
        
        public string title;
        
        protected Lib.Parameter<string> driver;
        protected Lib.Parameter<string> host;
        protected Lib.Parameter<int> port;
        protected Lib.Parameter<string> charset;
        protected Lib.Parameter<string> base_name;
        protected Lib.Parameter<string> username;
        protected Lib.Parameter<string> password;


        protected Lib.Database database;
        private Timer timer;

        #endregion

        #region CONSTRUCTOR
        public Service(Core parent, string name)
        {
            try
            {

                this.parent = parent;
                this.name = name;
                this.title = $"Service [{this.name}]";

                driver = new Lib.Parameter<string>($"SERVICE [{this.name}] DRIVER");
                host = new Lib.Parameter<string>($"SERVICE [{this.name}] HOST");
                port = new Lib.Parameter<int>($"SERVICE [{this.name}] PORT");
                charset = new Lib.Parameter<string>($"SERVICE [{this.name}] CHARSET");
                base_name = new Lib.Parameter<string>($"SERVICE [{this.name}] BASE NAME");
                username = new Lib.Parameter<string>($"SERVICE [{this.name}] USERNAME");
                password = new Lib.Parameter<string>($"SERVICE [{this.name}] PASSWORD");

                //timer = new Timer(Handler, null, 0, 60000);
                timer = new Timer(Handler, null, 0, 5000);

                logger.Info($"{title} started");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        #endregion

        #region DESTRUCTOR

        ~ Service()
        {
            logger.Info($"{title} stopped");
        }

        protected bool disposedValue;

        public virtual void Dispose(bool disposing) { }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion


        public void UpdateSettings(string driver, string host, int port, string charset, string base_name, string username, string password)
        {
            try
            {
                this.driver.Value = driver;
                this.host.Value = host;
                this.port.Value = port;
                this.charset.Value = charset;
                this.base_name.Value = base_name;
                this.username.Value = username;
                this.password.Value = password;

                if (database == null)
                    database = new Lib.Database(this.driver, this.host, this.port, this.charset, this.base_name, this.username, this.password);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }


        public virtual void Handler(object state) { }

    }
}
