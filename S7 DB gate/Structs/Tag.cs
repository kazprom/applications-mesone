using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace S7_DB_gate.Structs
{
    public class Tag: LibDBgate.Structs.Tag
    {

        [Field(Field.Etype.VarChar, 10, NN = true)]
        public string PLC_data_type { get; set; }

        [Field(Field.Etype.Int, NN = true)]
        public int Data_block_no { get; set; }

        [Field(Field.Etype.Int, NN = true)]
        public int Data_block_offset { get; set; }

        [Field(Field.Etype.TinyInt, NN = true)]
        public byte Bit_offset { get; set; }

        [Field(Field.Etype.VarChar, 15, NN = true)]
        public string Request_type { get; set; }


    }
}
