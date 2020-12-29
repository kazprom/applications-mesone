using Lib;

namespace LibMESone.Tables
{
    public class CBaseID
    {

        [Field(TYPE = Field.EDoctrine.UnsignedBigInteger, PK = true, AI = true, NN = true)]
        public ulong Id { get; set; }
     
    }
}
