using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace KingPigeonS272_DB_gate.Tables
{
    class CClient: LibMESone.Tables.CBaseNE
    {

        static public string TableName = "clients";


        [Field(TYPE = Field.EDoctrine.UnsignedBigInteger, NN = true)]
        public ulong Sockets_id { get; set; }


        [Field(TYPE = Field.EDoctrine.String, SIZE = 17, NN = true)]
        public string Imei { get; set; }


        [Field(TYPE = Field.EDoctrine.UnsignedSmallInteger, NN = true)]
        public ushort Timeout_m { get; set; }


    }
}
