using Lib;

namespace S7_DB_gate.Tables
{
    public class CClient : LibPlcDBgate.Tables.CClient
    {

        [Field(TYPE = Field.EDoctrine.String, SIZE = 10, NN = true)]
        public string Cpu_type { get; set; }

        [Field(TYPE = Field.EDoctrine.SmallInt, NN = true)]
        public ushort Rack { get; set; }

        [Field(TYPE = Field.EDoctrine.SmallInt, NN = true)]
        public ushort Slot { get; set; }

    }
}
