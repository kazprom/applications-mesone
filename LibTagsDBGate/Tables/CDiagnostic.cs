using Lib;

namespace LibPlcDBgate.Tables
{
    public class CDiagnostic: LibDBgate.Tables.CDiagnostic
    {

        [Field(TYPE = Field.EDoctrine.BigInt, NN = true, UQ = true)]
        public ulong Clients_id { get; set; }


    }
}
