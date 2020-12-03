using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSV_DB_gate.Tables
{
    public class CField : LibMESone.Tables.CBaseE
    {

        static public string TableName = "fields";

        [Field(TYPE = Field.Etype.BigInt, NN = true)]
        public ulong Converters_id { get; set; }

        [Field(TYPE = Field.Etype.VarChar, SIZE = 255, NN = true)]
        public string Name_src { get; set; }
        
        [Field(TYPE = Field.Etype.VarChar, SIZE = 255, NN = true)]
        public string Name_dst { get; set; }

        [Field(TYPE = Field.Etype.VarChar, SIZE = 15, NN = true)]
        public string Data_type { get; set; }


    }
}
