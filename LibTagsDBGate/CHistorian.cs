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

                Dictionary<DateTime, List<Tables.CHisValue>> history = new Dictionary<DateTime, List<Tables.CHisValue>>();

                lock (buffer)
                {
                    while (buffer.Count > 0)
                    {
                        Tables.CHisValue item = buffer.Dequeue();

                        if (!history.ContainsKey(item.Timestamp))
                            history.Add(item.Timestamp, new List<Tables.CHisValue>());

                        history[item.Timestamp].Add(item);

                    }
                }

                if (DB != null)
                {

                    foreach (var table in history)
                    {
                        string table_name = Tables.CHisValue.GetTableName(table.Key);

                        if (DB.CheckExistTable(table_name) == false)
                            DB.CreateTable<Tables.CHisValue>(table_name);

    
                    foreach (var row in table.Value)
                        {
                            DB.Insert<Tables.CHisValue>(table_name, row);
                        }
                    }

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
