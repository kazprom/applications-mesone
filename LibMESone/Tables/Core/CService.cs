using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;

namespace LibMESone.Tables.Core
{
    public class CService : CBaseNE
    {

        static public string TableName = "services";

        [Field(TYPE = Field.Etype.BigInt, UN = true)]
        public ulong Databases_id { get; set; }

        [Field(TYPE = Field.Etype.BigInt, UN = true)]
        public ulong Service_types_id { get; set; }

     
    }
}
