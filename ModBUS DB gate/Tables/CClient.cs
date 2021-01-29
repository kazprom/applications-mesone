using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModBUS_DB_gate.Tables
{
    class CClient : LibPlcDBgate.Tables.CClient
    {

        [Field(TYPE = Field.EDoctrine.String, SIZE = 3, NN = true)]
        public string Protocol { get; set; }

        [Field(TYPE = Field.EDoctrine.UnsignedTinyInteger, NN = true)]
        public ushort Address { get; set; }


        [Field(TYPE = Field.EDoctrine.Boolean, NN = true)]
        public bool Sb { get; set; }

        [Field(TYPE = Field.EDoctrine.Boolean, NN = true)]
        public bool Sw { get; set; }


    }
}
