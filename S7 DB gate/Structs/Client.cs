using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace S7_DB_gate.Structs
{
    public class Client:LibDBgate.Structs.Client
    {

        [Field(TYPE = Field.Etype.VarChar, SIZE = 10, NN = true)]
        public string Cpu_type { get; set; }

        [Field(TYPE = Field.Etype.SmallInt, NN = true, UN = true)]
        public ushort Rack { get; set; }

        [Field(TYPE = Field.Etype.SmallInt, NN = true, UN = true)]
        public ushort Slot { get; set; }

    }
}
