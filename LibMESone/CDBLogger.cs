using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace LibMESone
{
    public class CDBLogger : CSrvDB
    {

        public Lib.CBuffer<Tables.Custom.CLogMessage> Buffer { get; set; } = new Lib.CBuffer<Tables.Custom.CLogMessage>(100);

        private DateTime log_ts = default;

        public CDBLogger()
        {
            CycleRate = 5000;
        }


        public void Message(DateTime ts, string message)
        {
            try
            {
                Buffer.Enqueue(new Tables.Custom.CLogMessage() { Timestamp = ts, Message = message });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }


        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {

            try
            {

                Tables.Custom.CLogMessage data = null;

                if (DB != null)
                {
                    while (Buffer.Count > 0)
                    {

                        data = Buffer.Dequeue();


                        if (log_ts == default || log_ts != data.Timestamp)
                        {

                            switch (DB.CheckExistTable(Tables.Custom.CLogMessage.GetTableName(data.Timestamp)))
                            {
                                case false:
                                    if (DB.CreateTable<Tables.Custom.CLogMessage>(Tables.Custom.CLogMessage.GetTableName(data.Timestamp)))
                                    {
                                        log_ts = data.Timestamp;
                                    }
                                    break;
                            }

                        }

                        DB.Insert(Tables.Custom.CLogMessage.GetTableName(data.Timestamp), data);
                    }
                }
            }
            finally
            {

            }

            base.Timer_Handler(sender, e);
        }

    }
}
