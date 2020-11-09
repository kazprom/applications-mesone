using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibDBgate.Structs
{
    public class History : LibMESone.Structs.BaseID
    {

        [Field(TYPE = Field.Etype.BigInt, NN = true, UN = true)]
        public long Tags_id { get; set; }

        [Field(TYPE = Field.Etype.TimeStamp, SIZE = 3, NN = true)]
        public DateTime Timestamp { get; set; }

        [Field(TYPE = Field.Etype.Binary, SIZE = 8, NN = true)]
        public byte[] Value { get; set; }

        [Field(TYPE = Field.Etype.SmallInt, NN = true)]
        public short Quality { get; set; }


        public static string GetTableName(DateTime ts)
        {
            return $"t_{ts:yyyy_MM_dd_HH}";
        }

    }
}
