using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSV_DB_gate.Tables
{
    public class CConverter : LibMESone.Tables.CBaseNE
    {

        static public string TableName = "converters";

        [Field(TYPE = Field.EDoctrine.String, SIZE = 255, NN = true)]
        public string File_path { get; set; }

        [Field(TYPE = Field.EDoctrine.Boolean,  NN = true)]
        public bool File_delete { get; set; }

        [Field(TYPE = Field.EDoctrine.SmallInt, NN = true)]
        public uint File_depth_his { get; set; }

        [Field(TYPE = Field.EDoctrine.Boolean, NN = true)]
        public bool Table_clear { get; set; }

        [Field(TYPE = Field.EDoctrine.DateTime, NN = true)]
        public DateTime Start_timestamp { get; set; }

        [Field(TYPE = Field.EDoctrine.Integer, NN = true)]
        public uint Frequency_sec { get; set; }

        [Field(TYPE = Field.EDoctrine.Integer, NN = true)]
        public uint Timeout_sec { get; set; }

        [Field(TYPE = Field.EDoctrine.Boolean, NN = true)]
        public bool Has_header_record { get; set; }

        [Field(TYPE = Field.EDoctrine.String, SIZE = 5, NN = true)]
        public string Delimiter { get; set; }

        [Field(TYPE = Field.EDoctrine.String, SIZE = 1, NN = true)]
        public char Quote { get; set; }

        [Field(TYPE = Field.EDoctrine.Boolean, NN = true)]
        public bool Quotes_ignore { get; set; }

        [Field(TYPE = Field.EDoctrine.Boolean, NN = true)]
        public bool Detect_column_count_changes { get; set; }

        [Field(TYPE = Field.EDoctrine.Boolean, NN = true)]
        public bool Replaceable { get; set; }
    }
}
