using Lib;

namespace LibPlcDBgate.Structs
{
    public class Diagnostic: LibDBgate.Structs.CDiagnostic
    {

        [Field(TYPE = Field.Etype.BigInt, NN = true, UN = true, UQ = true)]
        public ulong Clients_id { get; set; }


    }
}
