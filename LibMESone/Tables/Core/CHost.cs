using Lib;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Text;

namespace LibMESone.Tables.Core
{
    public class CHost : CBaseNE
    {

        static public string TableName = "hosts";

        [Field(TYPE = Field.Etype.VarChar, SIZE = 15, NN = true)]
        public string Ip { get; set; }

    }
}
