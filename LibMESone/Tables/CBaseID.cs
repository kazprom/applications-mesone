using Lib;

namespace LibMESone.Tables
{
    public class CBaseID
    {

        [Field(TYPE = Field.Etype.BigInt, PK = true, AI = true, NN = true, UN = true)]
        public ulong Id { get; set; }
     
    }
}
