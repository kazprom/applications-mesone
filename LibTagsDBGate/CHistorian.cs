using LibMESone;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace LibPlcDBgate
{
    public class CHistorian : CSrvCyc
    {
        public override void LoadSetting(ISetting setting)
        {

        }

        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {


            base.Timer_Handler(sender, e);  
        }

        public override void Dispose(bool disposing)
        {



            base.Dispose(disposing);    
        }

    }
}
