using LibMESone.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace LibMESone
{
    public class Service : IDisposable
    {

        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region VARIABLES

        protected string name;
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
        public Service(string name)
        {
            try
            {
                logger.Info(name);

                this.name = name;

                driver = new Lib.Parameter<string>($"SERVICE [{this.name}] DRIVER");
                host = new Lib.Parameter<string>($"SERVICE [{this.name}] HOST");
                port = new Lib.Parameter<int>($"SERVICE [{this.name}] PORT");
                charset = new Lib.Parameter<string>($"SERVICE [{this.name}] CHARSET");
                base_name = new Lib.Parameter<string>($"SERVICE [{this.name}] BASE NAME");
                username = new Lib.Parameter<string>($"SERVICE [{this.name}] USERNAME");
                password = new Lib.Parameter<string>($"SERVICE [{this.name}] PASSWORD");

                timer = new Timer(Handler, null, 0, 6000);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        #endregion


        #region DESTRUCTOR

        protected bool disposedValue;

        public virtual void Dispose(bool disposing) { }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
            logger.Info(name);
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
                throw new Exception("Error update settings", ex);
            }
        }


        public virtual void Handler(object state) { }

    }
}
