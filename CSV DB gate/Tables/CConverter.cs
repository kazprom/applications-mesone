using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSV_DB_gate.Tables
{
    public class CConverter : LibMESone.Tables.CBaseNE
    {

        static public string TableName = "converters";

        [Field(TYPE = Field.Etype.VarChar, SIZE = 255, NN = true)]
        public string File_path { get; set; }

        [Field(TYPE = Field.Etype.TinyInt, SIZE = 1,  NN = true)]
        public bool File_delete { get; set; }

        [Field(TYPE = Field.Etype.TinyInt, SIZE = 1, NN = true)]
        public bool Table_clear { get; set; }

        [Field(TYPE = Field.Etype.TimeStamp, NN = true)]
        public DateTime Start_timestamp { get; set; }

        [Field(TYPE = Field.Etype.Int, NN = true)]
        public uint Frequency_sec { get; set; }

        [Field(TYPE = Field.Etype.Int, NN = true)]
        public uint Timeout_sec { get; set; }



    }
}
