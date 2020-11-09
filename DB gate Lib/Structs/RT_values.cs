﻿using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LibDBgate.Structs
{
    public class RT_values
    {

        [Field(TYPE = Field.Etype.BigInt, UN = true, NN = true, UQ = true)]
        public long Tags_id { get; set; }

        [Field(TYPE = Field.Etype.TimeStamp, SIZE = 3)]
        public DateTime Timestamp { get; set; }

        [Field(TYPE = Field.Etype.Binary, SIZE = 8)]
        public byte[] Value_raw { get; set; }

        [Field(TYPE = Field.Etype.VarChar, SIZE = 255)]
        public string Value_str { get; set; }

        [Field(TYPE = Field.Etype.TinyInt, UN = true)]
        public byte Quality { get; set; }
       
    }
}
