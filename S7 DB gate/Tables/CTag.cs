using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace S7_DB_gate.Tables
{
    public class CTag : LibPlcDBgate.Tables.CTag
    {


        [Field(TYPE = Field.EDoctrine.String, SIZE = 10, NN = true)]
        public string PLC_data_type { get; set; }

        [Field(TYPE = Field.EDoctrine.Integer, NN = true)]
        public int Data_block_no { get; set; }

        [Field(TYPE = Field.EDoctrine.Integer, NN = true)]
        public int Data_block_offset { get; set; }

        [Field(TYPE = Field.EDoctrine.SmallInt, NN = true)]
        public byte Bit_offset { get; set; }

        [Field(TYPE = Field.EDoctrine.String, SIZE = 15, NN = true)]
        public string Request_type { get; set; }

    }
}
