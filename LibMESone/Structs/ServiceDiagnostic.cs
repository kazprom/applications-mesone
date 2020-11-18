using Lib;
using System;
using System.Data;
using System.Data.Odbc;
using System.Linq;

namespace LibMESone.Structs
{
    public class ServiceDiagnostic: BaseID
    {
        static public string TableName = "service_diagnostics";

        [Field(TYPE = Field.Etype.BigInt, UN = true, UQ = true, NN = true)]
        public ulong Service_types_id { get; set; }

        [Field(TYPE = Field.Etype.VarChar, SIZE = 30)]
        public string Version { get; set; }

        [Field(TYPE = Field.Etype.TimeStamp)]
        public DateTime Sys_ts { get; set; }

    }
}
