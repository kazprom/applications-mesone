using LibMESone;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace LibPlcDBgate
{
    public class CHistoryCleaner : CSrvDB
    {
        private CHistorian historian;

        public CHistoryCleaner(CHistorian cHistorian)
        {
            historian = cHistorian;
            CycleRate = 60000;
        }

        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {

            try
            {

                if(historian != null && historian.DB != null)
                {

                    foreach (var table in DB.GetListTables(Tables.CHisValue.TablePrefix))
                    {



                    }

                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            base.Timer_Handler(sender, e);
        }

    }
}
