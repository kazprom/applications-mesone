using Lib;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Text;

namespace LibMESone.Structs
{
    public class Databases : BaseNE
    {

        [Field(TYPE = Field.Etype.VarChar, SIZE = 255)]
        public string Database { get; set; }

        [Field(TYPE = Field.Etype.VarChar, SIZE = 255)]
        public string Driver { get; set; }

        [Field(TYPE = Field.Etype.BigInt, UN = true)]
        public ulong Hosts_id { get; set; }

        [Field(TYPE = Field.Etype.Int, UN = true)]
        public uint Port { get; set; }

        [Field(TYPE = Field.Etype.VarChar, SIZE = 255)]
        public string Charset { get; set; }

        [Field(TYPE = Field.Etype.VarChar, SIZE = 255)]
        public string Username { get; set; }

        [Field(TYPE = Field.Etype.VarChar, SIZE = 255)]
        public string Password { get; set; }


    }
}
