using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LibDBgate
{
    public class Group : IDisposable
    {

        #region VARIABLES

        protected NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Dictionary<ulong, Tag> Tags = new Dictionary<ulong, Tag>();

        private Timer timer;

        #endregion

        #region PROPERTIES


        public string Title { get; private set; }

        public Client Parent { get; private set; }

        public ushort Rate { get; private set; }


        #endregion

        #region CONSTRUCTOR

        public Group(Client parent, ushort rate)
        {
            try
            {

                Parent = parent;
                Rate = rate;

                Title = $"{parent.Title} Group [{rate}]";

                timer = new Timer(Handler, null, 0, rate);

                logger.Info($"{Title}. Created");

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

        }

        #endregion

        #region DESTRUCTOR

        ~Group()
        {
        }

        protected bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    WaitHandle h = new AutoResetEvent(false);
                    timer.Dispose(h);
                    h.WaitOne();

                    foreach (Tag tag in Tags.Values)
                    {
                        tag.Dispose();
                    }
                    Tags.Clear();
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


        public virtual void LoadTags(dynamic tags) { }

        public virtual void Handler(object state) { }



    }
}
