using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibDBgate.Tables
{
    public class Setting:LibMESone.Tables.BaseID
    {

        [Field(Field.Etype.VarChar, 50, NN = true)]
        public string Key { get; set; }

        [Field(Field.Etype.VarChar, 255, NN = true)]
        public string Value { get; set; }


    }
}
