using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace OPC_DB_gate_server
{
    class HandlerData
    {

        #region VARIABLES

        private Lib.Buffer<LibDBgate.TagData> buffer;

        private RT_values rt_values;
        private History history;
        private Application application;

        Lib.Parameter<bool> rt_values_enable;
        Lib.Parameter<bool> history_enable;

        private Thread thread;
        private bool execution = true;

        #endregion



        public HandlerData(Lib.Buffer<LibDBgate.TagData> buffer,
                           RT_values rt_values, Lib.Parameter<bool> rt_values_enable,
                           History history, Lib.Parameter<bool> history_enable,
                           Application application)
        {
            this.rt_values = rt_values;
            this.rt_values_enable = rt_values_enable;

            this.history = history;
            this.history_enable = history_enable;

            this.application = application;

            this.buffer = buffer;
            this.buffer.HalfEvent += BufferEmptier;

            thread = new Thread(new ThreadStart(Handler)) { IsBackground = true, Name = "Buffer handler" };
            thread.Start();
        }


        private void Handler()
        {
            while (execution)
            {
                try
                {
                    BufferEmptier();
                    application.Put(Application.EKeys.CLOCK, DateTime.Now.ToString());

                }
                catch (Exception ex)
                {
                    Lib.Message.Make("Error buffer handler", ex);
                }
                Thread.Sleep(2000);
            }
        }


        private void BufferEmptier()
        {
            try
            {
                lock (buffer)
                {
                    while (buffer.Count > 0)
                    {
                        LibDBgate.TagData tag = buffer.Dequeue();

                        if (rt_values_enable.Value)
                            rt_values.Put(tag);

                        if (history_enable.Value)
                            history.Put(tag);

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error buffer emtier", ex);
            }
        }
    }
}
