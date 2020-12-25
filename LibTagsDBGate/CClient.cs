using LibMESone;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

namespace LibPlcDBgate
{
    public class CClient : LibDBgate.CSrvSub
    {
        /*
        #region CONSTANTS

#if DEBUG
        private const int period = 5000;
#else
        private const int period = 60000;
#endif

        #endregion

        #region VARIABLE

        public Dictionary<ulong, CGroup> Groups { get; set; }

        #endregion

        #region CONSTRUCTOR

        public CClient()
        {
            Groups = new Dictionary<ulong, CGroup>();
        }

        #endregion

        #region DESTRUCTOR

        public override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (CGroup group in Groups.Values)
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
        */


        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {



            base.Timer_Handler(sender, e);
        }

    }
}
