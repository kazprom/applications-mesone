using Lib;
using System;
using System.Data;
using System.Data.Odbc;
using System.Linq;

namespace LibMESone.Structs
{
    public class Application
    {
        [Field(Field.Etype.VarChar, 50, pk: true)]
        public string Key { get; set; }

        [Field(Field.Etype.VarChar, 255)]
        public string Value { get; set; }

    }
}
