using LibMESone;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

namespace LibPlcDBgate
{
    public class CGroup : CSrvCyc
    {

        public Structs.CGroup Settings { get; set; }

        public override void LoadSetting(ISetting setting)
        {
            Settings = setting as Structs.CGroup;

            CycleRate = Settings.Rate;

        }

        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {



            base.Timer_Handler(sender, e);  
        }

    }
}
