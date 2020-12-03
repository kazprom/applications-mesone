using Lib;

namespace LibDBgate.Tables
{
    public class CDiagnostic: LibMESone.Tables.CBaseID
    {

        static public string TableName = "diagnostics";


        [Field(TYPE = Field.Etype.VarChar, SIZE = 50)]
        public string State { get; set; }

        [Field(TYPE = Field.Etype.Text)]
        public string Message { get; set; }



    }
}
