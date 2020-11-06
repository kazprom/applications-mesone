using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LibDBgate.Structs
{
    public class RT_values
    {

        [Field(Field.Etype.BigInt, UN = true, NN = true, UQ = true)]
        public long Tags_id { get; set; }

        [Field(Field.Etype.TimeStamp, 3)]
        public DateTime Timestamp { get; set; }

        [Field(Field.Etype.Binary, 8)]
        public byte[] Value_raw { get; set; }

        [Field(Field.Etype.VarChar, 255)]
        public string Value_str { get; set; }

        [Field(Field.Etype.TinyInt, UN = true)]
        public byte Quality { get; set; }
       
    }
}
