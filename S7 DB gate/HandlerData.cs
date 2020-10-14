using System;
using System.Threading;

namespace S7_DB_gate
{
    public class HandlerData
    {
        private Lib.Buffer<LibDBgate.TagData> buffer;
        private LibDBgate.RT_values rt_values;
        private LibDBgate.History history;
        private Lib.Application application;

        private Thread thread;
        private bool execution = true;

        public HandlerData(Lib.Buffer<LibDBgate.TagData> buffer,
                           LibDBgate.RT_values rt_values,
                           LibDBgate.History history,
                           Lib.Application application)
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
                    application.Put(Lib.Application.EKeys.CLOCK, DateTime.Now.ToString());

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

                        if (true)
                            rt_values.Put(tag);

                        if (true)
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