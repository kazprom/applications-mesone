using System;
using System.Threading;

namespace LibMESone
{
    public class Service : IDisposable
    {

        #region CONSTANTS

#if DEBUG
        private const int period = 5000;
#else
        private const int period = 60000;
#endif

        #endregion

        #region VARIABLES

        protected NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private Timer timer_database_read;

        #endregion

        #region PROPERTIES

        public Core Parent { get; private set; }

        public string Title { get; private set; }

        public ulong ID { get; private set; }

        public string Name { get; private set; }

        public Lib.Database Database { get; set; }

        #endregion

        #region CONSTRUCTOR
        public Service(Core parent, ulong id)
        {
            try
            {

                Parent = parent;
                ID = id;
                Title = $"Service [{ID}]";

                timer_database_read = new Timer(DatabaseReadHandler, null, 0, period);

                logger.Info($"{Title}. Created");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        #endregion

        #region DESTRUCTOR

        ~Service()
        {
        }

        private bool disposedValue;

        public virtual void Dispose(bool disposing) {

            if (!disposedValue)
            {
                if (disposing)
                {
                    WaitHandle h = new AutoResetEvent(false);
                    timer_database_read.Dispose(h);
                    h.WaitOne();

                    Database = null;
                }

                disposedValue = true;
            }

        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
            logger.Info($"{Title}. Disposed");
        }

        #endregion

        #region PUBLICS
        public void LoadDatabaseSettings(ulong id, string name, string driver, string host, uint port, string charset, string base_name, string username, string password)
        {
            try
            {
                if (Database == null)
                    Database = new Lib.Database(id);

                Database.LoadSettings(name, driver, host, port, charset, base_name, username, password);

            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Title}. Load Database settings");
            }
        }

        public virtual void DatabaseReadHandler(object state) { }

        #endregion

    }
}
