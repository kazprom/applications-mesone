using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;

namespace LibMESone.Structs
{
    public class Services : BaseNE
    {
        [Field(TYPE = Field.Etype.BigInt, UN = true)]
        public ulong Databases_id { get; set; }

        [Field(TYPE = Field.Etype.BigInt, UN = true)]
        public ulong Service_types_id { get; set; }

     
    }
}
