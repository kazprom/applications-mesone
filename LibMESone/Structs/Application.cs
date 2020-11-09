using Lib;
using System;
using System.Data;
using System.Data.Odbc;
using System.Linq;

namespace LibMESone.Structs
{
    public class Application
    {
        [Field(TYPE = Field.Etype.VarChar, SIZE = 50, PK = true)]
        public string Key { get; set; }

        [Field(TYPE = Field.Etype.VarChar, SIZE = 255)]
        public string Value { get; set; }

    }
}
