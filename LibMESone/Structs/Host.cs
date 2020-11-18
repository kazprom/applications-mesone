using Lib;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Text;

namespace LibMESone.Structs
{
    public class Host:BaseNE
    {

        [Field(TYPE = Field.Etype.VarChar, SIZE = 15, NN = true)]
        public string Ip { get; set; }

    }
}
