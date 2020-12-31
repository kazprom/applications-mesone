using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace LibMESone
{
    public class CDBLogCleaner : CSrvDB
    {
        private CDBLogger db_logger;
        private DateTime log_ts = default;

        public CDBLogCleaner(CDBLogger cDBLogger)
        {
            db_logger = cDBLogger;
            CycleRate = 30000;
        }

        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {


            try
            {

                Tables.Custom.CLogMessage data = null;

                if (DB != null)
                {
                    while (db_logger.Buffer.Count > 0)
                    {

                        data = db_logger.Buffer.Dequeue();


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
