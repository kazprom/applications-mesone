using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LibPlcDBgate.Tables
{
    public class CRtValue : LibMESone.Tables.CBaseID
    {

        static public string TableName = "rt_values";

        [Field(TYPE = Field.EDoctrine.BigInt, NN = true, UQ = true)]
        public ulong Tags_id { get; set; }

        [Field(TYPE = Field.EDoctrine.DateTime, SIZE = 3)]
        public DateTime Timestamp { get; set; }

        [Field(TYPE = Field.EDoctrine.Blob)]
        public byte[] Value_raw { get; set; }

        [Field(TYPE = Field.EDoctrine.String, SIZE = 255)]
        public string Value_str { get; set; }

        [Field(TYPE = Field.EDoctrine.SmallInt)]
        public byte Quality { get; set; }
       
    }
}
