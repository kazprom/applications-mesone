using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibMESone.Tables.Custom
{
    public class CSetting: CBaseID
    {

        static public string TableName = "settings";

        [Field(TYPE = Field.EDoctrine.String, SIZE = 50, NN = true, UQ = true)]
        public string Key { get; set; }

        [Field(TYPE = Field.EDoctrine.String, SIZE = 255, NN = true)]
        public string Value { get; set; }


    }
}
