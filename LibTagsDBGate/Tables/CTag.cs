using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibPlcDBgate.Tables
{
    public class CTag : LibMESone.Tables.CBaseNE
    {

        static public string TableName = "tags";

        [Field(TYPE = Field.EDoctrine.UnsignedBigInteger, NN = true)]
        public ulong Clients_id { get; set; }

        [Field(TYPE = Field.EDoctrine.UnsignedSmallInteger, NN = true)]
        public ushort Rate { get; set; }

        [Field(TYPE = Field.EDoctrine.String, SIZE = 15, NN = true)]
        public string Data_type { get; set; }

        [Field(TYPE = Field.EDoctrine.Boolean, NN = true)]
        public bool RT_values_enabled { get; set; }

        [Field(TYPE = Field.EDoctrine.Boolean, NN = true)]
        public bool History_enabled { get; set; }


    }
}
