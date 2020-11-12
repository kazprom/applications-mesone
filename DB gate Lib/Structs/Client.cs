using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibDBgate.Structs
{
    public class Client : LibMESone.Structs.BaseNE
    {

        static public string TableName = "clients";

        [Field(TYPE = Field.Etype.VarChar, SIZE = 15, NN = true)]
        public string Ip { get; set; }

        [Field(TYPE = Field.Etype.SmallInt, NN = true, UN = true)]
        public uint Port { get; set; }




    }
}
