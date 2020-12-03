using Lib;

namespace LibMESone.Tables
{
    public class CBaseE : CBaseID
    {
        [Field(TYPE = Field.Etype.TinyInt, SIZE = 1, NN = true)]
        public bool Enabled { get; set; }


    }
}
