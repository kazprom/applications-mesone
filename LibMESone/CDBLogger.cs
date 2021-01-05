using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;

namespace LibMESone
{
    public class CDBLogger : CSrvDB
    {
        private const string title = "DBlogger";

        private CDBLogCleaner cleaner;
        private DateTime log_ts = default;


        public Lib.CBuffer<Tables.Custom.CLogMessage> Buffer { get; set; } = new Lib.CBuffer<Tables.Custom.CLogMessage>(100);


        public override Logger Logger
        {
            set
            {
                base.Logger = LogManager.GetLogger($"{value.Name}.{title}");
                cleaner.Logger = Logger;
            }
        }

        public CDBLogger()
        {
            cleaner = new CDBLogCleaner(this);
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
