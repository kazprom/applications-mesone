using Lib;

namespace LibMESone.Tables
{
    public class CBaseNE : CBaseID
    {
        [Field(TYPE = Field.Etype.VarChar, SIZE = 255, UQ = true, NN = true)]
        public string Name { get; set; }

        [Field(TYPE = Field.Etype.TinyInt,SIZE = 1, NN = true)]
        public bool Enabled { get; set; }

     
    }
}
