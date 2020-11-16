using LibDBgate.Structs;
using System;
using System.Collections.Generic;
using System.Threading;

namespace LibDBgate
{
    public class Client : IDisposable
    {

        #region CONSTANTS

#if DEBUG
        private const int period = 5000;
#else
        private const int period = 60000;
#endif

        #endregion

        #region VARIABLE

        protected NLog.Logger logger;
        public Dictionary<ushort, Group> Groups = new Dictionary<ushort, Group>();
        private Timer timer;

        #endregion

        #region PROPERTIES

        public string Title { get; private set; }

        public Service Parent { get; private set; }

        public ulong ID { get; private set; }

        public string Name { get; set; }

        public Diagnostic Diagnostic { get; private set; }

        #endregion

        #region CONSTRUCTOR

        public Client(Service parent, ulong id)
        {
            Parent = parent;
            ID = id;

            Diagnostic = new Diagnostic() { Clients_id = ID };

            Title = $"{Parent.Title} Client [{ID}]";

            timer = new Timer(TagsReader, null, 0, period);

            logger = NLog.LogManager.GetLogger(Title);

            logger.Info($"{Title}. Created");

        }

        #endregion

        #region DESTRUCTOR

        ~Client()
        {
        }


        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    WaitHandle h = new AutoResetEvent(false);
                    timer.Dispose(h);
                    h.WaitOne();

                    foreach (Group group in Groups.Values)
                    {
                        group.Dispose();
                    }
                    Groups.Clear();
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

        #region PUBLIC

        public virtual void TagsReader(object state) { }

        #endregion

    }
}
