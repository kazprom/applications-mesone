using System;
using System.Collections.Generic;
using System.Text;

namespace LibPlcDBgate.Structs
{
    public class CGroup : LibDBgate.Structs.CSetSUB
    {

        public ushort Rate { get; set; }


        public IEnumerable<CTag> Tags { get; set; }


    }
}
