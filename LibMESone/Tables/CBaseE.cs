using Lib;

namespace LibMESone.Tables
{
    public class CBaseE : CBaseID
    {
        [Field(TYPE = Field.EDoctrine.Boolean, NN = true)]
        public bool Enabled { get; set; }


    }
}
