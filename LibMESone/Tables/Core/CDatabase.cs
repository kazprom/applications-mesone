using Lib;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Text;

namespace LibMESone.Tables.Core
{
    public class CDatabase : CBaseNE
    {

        static public string TableName = "databases";

        [Field(TYPE = Field.EDoctrine.String, SIZE = 255)]
        public string Database { get; set; }

        [Field(TYPE = Field.EDoctrine.String, SIZE = 255)]
        public string Driver { get; set; }

        [Field(TYPE = Field.EDoctrine.BigInt, NN = true)]
        public ulong Hosts_id { get; set; }

        [Field(TYPE = Field.EDoctrine.Integer)]
        public uint Port { get; set; }

        [Field(TYPE = Field.EDoctrine.String, SIZE = 255)]
        public string Charset { get; set; }

        [Field(TYPE = Field.EDoctrine.String, SIZE = 255)]
        public string Username { get; set; }

        [Field(TYPE = Field.EDoctrine.String, SIZE = 255)]
        public string Password { get; set; }


    }
}
