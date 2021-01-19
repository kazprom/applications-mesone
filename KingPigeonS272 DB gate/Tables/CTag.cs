using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace KingPigeonS272_DB_gate.Tables
{
    class CTag: LibPlcDBgate.Tables.CTag
    {

        [Field(TYPE = Field.EDoctrine.String, SIZE = 4, NN = true)]
        public string Channel { get; set; }

    }
}
