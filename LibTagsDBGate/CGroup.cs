using LibMESone;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

namespace LibPlcDBgate
{
    public class CGroup : CSrvCyc
    {

        public override ulong Id
        {
            set
            {
                base.Id = value;
                Name = $"g{Id}";

                CycleRate = (uint)value;
            }
        }


        public virtual dynamic Tags { get; set; }


    }
}
