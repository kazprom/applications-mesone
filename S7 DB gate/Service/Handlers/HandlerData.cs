using System;
using System.Threading;

namespace S7_DB_gate.Handlers
{
    public class HandlerData
    {
        /*

        private Lib.Buffer<LibDBgate.TagData> buffer;
        private LibDBgate.Trt_values rt_values;
        private LibDBgate.HistoryFiller history;
        private LibMESone.Tables.Tapplication application;

        private Thread thread;
        private bool execution = true;

        public HandlerData(Lib.Buffer<LibDBgate.TagData> buffer,
                           LibDBgate.Trt_values rt_values,
                           LibDBgate.HistoryFiller history,
                           LibMESone.Tables.Tapplication application)
        {
            this.application = application;

            this.buffer = buffer;
            this.rt_values = rt_values;
            this.history = history;
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
                    application.Put(LibMESone.Tables.Tapplication.EKeys.CLOCK, DateTime.Now.ToString());

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

                        if (tag.settings.HasFlag(LibDBgate.TagData.ESettings.rt_value_enabled))
                            rt_values.Put(tag);

                        if (tag.settings.HasFlag(LibDBgate.TagData.ESettings.history_enabled))
                            history.Put(tag);

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error buffer emtier", ex);
            }
        }

        */
    }
}