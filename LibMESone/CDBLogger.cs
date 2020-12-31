using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace LibMESone
{
    public class CDBLogger : CSrvDB
    {

        private CDBLogCleaner cleaner;

        public Lib.CBuffer<Tables.Custom.CLogMessage> Buffer { get; set; } = new Lib.CBuffer<Tables.Custom.CLogMessage>(100);


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
        

    }
}
