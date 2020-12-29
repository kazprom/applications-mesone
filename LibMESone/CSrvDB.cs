using System;
using System.Collections.Generic;
using System.Text;

namespace LibMESone
{
    public class CSrvDB : CSrvCyc
    {

        #region CONSTANTS

#if DEBUG
        private const int period = 5000;
#else
        private const int period = 60000;
#endif

        #endregion

        public Dictionary<string, string> DBprops { get; set; } = new Dictionary<string, string>();

        public Lib.CDatabase DB { get; set; } = new Lib.CDatabase();


        public CSrvDB()
        {

            LoggerMaked += CSrvDB_LoggerMaked;

            CycleRate = period;

        }

        private void CSrvDB_LoggerMaked(NLog.Logger logger)
        {
            if (DB != null)
                DB.Logger = logger;
        }
    }
}
