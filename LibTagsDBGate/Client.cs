using System;
using System.Collections.Generic;
using System.Threading;

namespace LibPlcDBgate
{
    public class Client : LibDBgate.CSrvSub
    {

        #region CONSTANTS

#if DEBUG
        private const int period = 5000;
#else
        private const int period = 60000;
#endif

        #endregion

        #region VARIABLE

        public Dictionary<ushort, Group> Groups = new Dictionary<ushort, Group>();
        private Timer timer_DB;

        #endregion

        #region PROPERTIES

        public string Name { get; set; }

        #endregion

        #region CONSTRUCTOR

        public Client(Service parent, ulong id): base(parent, id)
        {

            Diagnostic = new Structs.Diagnostic() { Clients_id = ID };

            Title = $"{Parent.Title} Client [{ID}]";

            timer_DB = new Timer(DB_Handler, null, 0, period);

            logger = NLog.LogManager.GetLogger(Title);

            logger.Info($"{Title}. Created");

        }

        #endregion

        #region DESTRUCTOR

        ~Client()
        {
        }

        public override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    WaitHandle h = new AutoResetEvent(false);
                    timer_DB.Dispose(h);
                    h.WaitOne();

                    foreach (Group group in Groups.Values)
                    {
                        group.Dispose();
                    }
                    Groups.Clear();
                }
            }

            base.Dispose(disposing);
        }


        

        #endregion

        #region PUBLIC

        public virtual void DB_Handler(object state) { }

        #endregion

    }
}
