using System;
using System.Collections.Generic;
using System.Text;

namespace LibPlcDBgate.Structs
{
    public class CTag : LibDBgate.Structs.CSetSUB
    {
        public string Data_type { get; set; }

        public bool RT_values_enabled { get; set; }

        public bool History_enabled { get; set; }


    }
}
