using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModBUS_DB_gate.Tables
{
    class CTag : LibPlcDBgate.Tables.CTag
    {


        [Field(TYPE = Field.EDoctrine.String, SIZE = 20, NN = true)]
        public string Function { get; set; }

        [Field(TYPE = Field.EDoctrine.UnsignedSmallInteger, NN = true)]
        public ushort Address { get; set; }

    }
}
