using Lib;

namespace LibPlcDBgate.Tables
{
    public class CClient : LibMESone.Tables.CBaseNE
    {

        static public string TableName = "clients";

        [Field(TYPE = Field.EDoctrine.String, SIZE = 15, NN = true)]
        public string Ip { get; set; }

        [Field(TYPE = Field.EDoctrine.SmallInt, NN = true)]
        public uint Port { get; set; }

    }
}
