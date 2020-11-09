﻿using Lib;
using System.Data;
using System.Data.Odbc;

namespace LibDBgate.Structs
{
    public class Tag : LibMESone.Structs.BaseNE
    {

        [Field(TYPE = Field.Etype.BigInt, NN = true)]
        public long Clients_id { get; set; }

        [Field(TYPE = Field.Etype.SmallInt, NN = true)]
        public short Rate { get; set; }

        [Field(TYPE = Field.Etype.VarChar, SIZE = 15, NN = true)]
        public string Data_type { get; set; }

        [Field(TYPE = Field.Etype.TinyInt, SIZE = 1, NN = true)]
        public bool RT_values_enabled { get; set; }

        [Field(TYPE = Field.Etype.TinyInt, SIZE = 1, NN = true)]
        public bool History_enabled { get; set; }
    
    }
}
