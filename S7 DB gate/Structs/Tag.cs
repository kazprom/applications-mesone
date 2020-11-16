using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace S7_DB_gate.Structs
{
    public class Tag: LibDBgate.Structs.Tag
    {

        [Field(TYPE = Field.Etype.VarChar, SIZE = 10, NN = true)]
        public string PLC_data_type { get; set; }

        [Field(TYPE = Field.Etype.Int, NN = true, UN = true)]
        public int Data_block_no { get; set; }

        [Field(TYPE = Field.Etype.Int, NN = true, UN = true)]
        public int Data_block_offset { get; set; }

        [Field(TYPE = Field.Etype.TinyInt, NN = true, UN = true)]
        public byte Bit_offset { get; set; }

        [Field(TYPE = Field.Etype.VarChar, SIZE = 15, NN = true)]
        public string Request_type { get; set; }


    }
}
