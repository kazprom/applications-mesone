using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibMESone.Structs
{
    public class LogMessage : LibMESone.Structs.BaseID
    {

        [Field(TYPE = Field.Etype.TimeStamp, SIZE = 3, NN = true)]
        public DateTime Timestamp { get; set; }

        [Field(TYPE = Field.Etype.Text, NN = true)]
        public string Message { get; set; }

        public static string GetTableName(DateTime ts)
        {
            return $"l_{ts:yyyy_MM_dd}";
        }


    }
}
