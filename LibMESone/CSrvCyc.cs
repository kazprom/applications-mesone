using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace LibMESone
{
    public abstract class CSrvCyc : CSrv, IDisposable
    {

        #region VARIABLES

        private Timer timer;

        #endregion

        #region PROPERTIES

        private uint cycle_rate;

        public uint CycleRate
        {
            get { return cycle_rate; }
            set
            {
                if (cycle_rate != value)
                {
                    cycle_rate = value;

                    if (timer != null)
                    {
                        timer.Dispose();
                        timer = null;
                    }

                    if (cycle_rate > 0)
                    {
                        timer = new Timer(cycle_rate);
                        timer.Elapsed += Timer_Handler;
                        timer.AutoReset = false;
                        timer.Start();
                    }
                }
            }
        }

        #endregion

        public virtual void Timer_Handler(object sender, ElapsedEventArgs e)
        {
            try
            {
                timer.Start();
            }
            catch (Exception ex)
            {
                if (Logger != null) Logger.Error(ex);
            }
        }

        protected bool disposedValue;

        public virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CycleRate = 0;
                }

                disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }



    }
}
