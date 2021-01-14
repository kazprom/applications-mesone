using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace KingPigeonS272_DB_gate
{
    class CClient: LibMESone.CSrvCyc
    {

        #region VARIABLES

        private DateTime ts;

        #endregion


        #region PROPERTIES

        public byte[] Guid { get; set; }


        public ushort Timeout_m { get; set; }


        public dynamic Tags
        {
            set
            {
                ;
            }
        }

        #endregion



        public CClient()
        {
            try
            {
                CycleRate = 20000;
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



            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            base.Timer_Handler(sender, e);
        }


    }
}
