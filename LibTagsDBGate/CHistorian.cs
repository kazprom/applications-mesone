using LibMESone;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace LibPlcDBgate
{
    public class CHistorian : CSrvDB
    {

        private const string title = "HISTORIAN";

        private CHistoryCleaner cleaner;
        private Lib.CBuffer<Tables.CHisValue> buffer = new Lib.CBuffer<Tables.CHisValue>(1000);

        public override Logger Logger
        {
            set
            {
                base.Logger = LogManager.GetLogger($"{value.Name}.{title}");
                cleaner.Logger = Logger;
            }
        }

        public CHistorian()
        {

            cleaner = new CHistoryCleaner(this);

            buffer.FullEvent += Buffer_FullEvent;

            CycleRate = 10000;

        }

        private void Buffer_FullEvent(Tables.CHisValue obj)
        {
            Logger.Warn("Buffer is full");
        }

        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {

            try
            {

                while (buffer.Count > 0)
                {
                    buffer.Dequeue();
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


            base.Timer_Handler(sender, e);
        }


        public void Put(ulong id, DateTime ts, byte[] value, byte quality)
        {
            lock (buffer)
            {
                buffer.Enqueue(new Tables.CHisValue()
                {
                    Tags_id = id,
                    Timestamp = ts,
                    Value = value,
                    Quality = quality
                });
            }
        }

    }
}
