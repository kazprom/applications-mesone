using Lib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace OPC_DB_gate_server
{
    class BufferHandler
    {

        private Lib.Buffer<OPC_DB_gate_Lib.TagData> buffer;
        private RT_values rt_values;
        private History history;
        private Thread thread;
        private bool execution = true;


        public BufferHandler(Lib.Buffer<OPC_DB_gate_Lib.TagData> buffer,
                             RT_values rt_values,
                             History history)
        {
            this.rt_values = rt_values;
            this.history = history;

            this.buffer = buffer;
            this.buffer.HalfEvent += BufferEmptier;

            thread = new Thread(new ThreadStart(Handler)) { IsBackground = true, Name = "Buffer handler" };
            thread.Start();
        }


        private void Handler()
        {
            while(execution)
            {
                try
                {
                    BufferEmptier();
                }
                catch (Exception ex)
                {
                    Logger.WriteMessage("Error buffer handler",ex);
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
                        OPC_DB_gate_Lib.TagData tag = buffer.Dequeue();

                        rt_values.Put(tag);
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
