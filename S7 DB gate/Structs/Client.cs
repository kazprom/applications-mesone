using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace S7_DB_gate.Structs
{
    public class Client:LibDBgate.Structs.Client
    {

        [Field(Field.Etype.VarChar, 10, NN = true)]
        public string Cpu_type { get; set; }

        [Field(Field.Etype.SmallInt, NN = true)]
        public short Rack { get; set; }

        [Field(Field.Etype.SmallInt, NN = true)]
        public short Slot { get; set; }

    }
}
