using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibMESone.Structs
{
    public class Setting:LibMESone.Structs.BaseID
    {

        static public string TableName = "settings";

        [Field(TYPE = Field.Etype.VarChar, SIZE = 50, NN = true, UQ = true)]
        public string Key { get; set; }

        [Field(TYPE = Field.Etype.VarChar, SIZE = 255, NN = true)]
        public string Value { get; set; }


    }
}
