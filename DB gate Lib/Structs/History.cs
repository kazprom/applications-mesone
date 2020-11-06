using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibDBgate.Structs
{
    public class History 
    {

        [Field(Field.Etype.BigInt, NN = true)]
        public long Tags_id { get; set; }

        [Field(Field.Etype.TimeStamp, 3, NN = true)]
        public DateTime Timestamp { get; set; }

        [Field(Field.Etype.Binary, 8, NN = true)]
        public byte[] Value { get; set; }

        [Field(Field.Etype.SmallInt, NN = true)]
        public short Quality { get; set; }


        public static string GetTableName(DateTime ts)
        {
            return $"t_{ts:yyyy_MM_dd_HH}";
        }

    }
}
