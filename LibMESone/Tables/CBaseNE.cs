using Lib;

namespace LibMESone.Tables
{
    public class CBaseNE : CBaseID
    {
        [Field(TYPE = Field.EDoctrine.String, SIZE = 255, UQ = true, NN = true)]
        public string Name { get; set; }

        [Field(TYPE = Field.EDoctrine.Boolean, NN = true)]
        public bool Enabled { get; set; }

     
    }
}
