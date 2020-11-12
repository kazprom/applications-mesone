using Lib;

namespace LibDBgate.Structs
{
    public class Diagnostic: LibMESone.Structs.BaseID
    {

        static public string TableName = "diagnostics";


        [Field(TYPE = Field.Etype.BigInt, NN = true, UN = true, UQ = true)]
        public ulong Clients_id { get; set; }


        [Field(TYPE = Field.Etype.VarChar, SIZE = 50)]
        public string State { get; set; }

        [Field(TYPE = Field.Etype.Text)]
        public string Message { get; set; }



    }
}
