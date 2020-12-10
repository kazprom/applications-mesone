using Lib;

namespace LibDBgate.Tables
{
    public class CDiagnostic: LibMESone.Tables.CBaseID
    {

        static public string TableName = "diagnostics";


        [Field(TYPE = Field.EDoctrine.String, SIZE = 50)]
        public string State { get; set; }

        [Field(TYPE = Field.EDoctrine.String)]
        public string Message { get; set; }



    }
}
