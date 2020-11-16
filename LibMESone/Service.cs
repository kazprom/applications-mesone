using System;
using System.Threading;
using System.Threading.Tasks;

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

        protected NLog.Logger logger;
        private Timer timer_database_read;
        private Lib.Buffer<Structs.LogMessage> log_buf;
        private DateTime log_ts = default;

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

                log_buf = new Lib.Buffer<Structs.LogMessage>(100, 5000);
                log_buf.CyclicEvent += LogDataHandler;
                log_buf.HalfEvent += LogDataHandler;

                var configuration = NLog.LogManager.Configuration;
                var target = new NLog.Targets.MethodCallTarget(Title, (logEvent, parms) =>
                {
                    log_buf.Enqueue(new Structs.LogMessage() { Timestamp = logEvent.TimeStamp, Message = $"{logEvent.Level} {logEvent.Message} {logEvent.Exception}" });
                });


                configuration.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target, Title + "*");
                NLog.LogManager.Configuration = configuration;


                logger = NLog.LogManager.GetLogger(Title);

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

        public virtual void Dispose(bool disposing)
        {

            if (!disposedValue)
            {
                if (disposing)
                {
                    WaitHandle h = new AutoResetEvent(false);
                    timer_database_read.Dispose(h);
                    h.WaitOne();

                    LogDataHandler();

                    var configuration = NLog.LogManager.Configuration;
                    configuration.RemoveRuleByName(Title);
                    NLog.LogManager.Configuration = configuration;

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
                    Database = new Lib.Database(id, logger);

                Database.LoadSettings(name, driver, host, port, charset, base_name, username, password);

            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Title}. Load Database settings");
            }
        }

        public virtual void DatabaseReadHandler(object state) { }

        #endregion

        #region PRIVATES

        private async void LogDataHandler()
        {
            await Task.Run(() =>
            {

                try
                {

                    Structs.LogMessage data = null;

                    if (Database != null)
                    {
                        while (log_buf.Count > 0)
                        {

                            if (data == null)
                            {
                                data = log_buf.Dequeue();


                                if (log_ts == default || log_ts != data.Timestamp)
                                {
                                    if (!Database.CheckExistTable(Structs.LogMessage.GetTableName(data.Timestamp)))
                                    {
                                        if (Database.CreateTable<Structs.LogMessage>(Structs.LogMessage.GetTableName(data.Timestamp)))
                                        {
                                            log_ts = data.Timestamp;
                                        }
                                    }
                                }

                                if (Database.Insert(Structs.LogMessage.GetTableName(data.Timestamp), data))
                                {
                                    data = null;
                                }
                                else
                                {
                                    Thread.Sleep(1000);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"{Title}. Log data handler");
                }
            });
        }


        #endregion

    }
}
