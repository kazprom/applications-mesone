using Lib;
using System;
using System.Data;
using System.Data.Odbc;
using System.Linq;

namespace LibMESone.Tables.Core
{
    public class CServiceDiagnostic: CBaseID
    {
        static public string TableName = "service_diagnostics";

        [Field(TYPE = Field.EDoctrine.BigInt, UQ = true, NN = true)]
        public ulong Service_types_id { get; set; }

        [Field(TYPE = Field.EDoctrine.String, SIZE = 30)]
        public string Version { get; set; }

        [Field(TYPE = Field.EDoctrine.DateTime)]
        public DateTime Sys_ts { get; set; }

    }
}
