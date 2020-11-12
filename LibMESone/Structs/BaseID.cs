using Lib;

namespace LibMESone.Structs
{
    public class BaseID
    {

        [Field(TYPE = Field.Etype.BigInt, PK = true, AI = true, NN = true, UN = true)]
        public ulong Id { get; set; }
     
    }
}
