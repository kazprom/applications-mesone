using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibDBgate.Tables
{
    public class Client : LibMESone.Tables.BaseNE
    {



        [Field(Field.Etype.VarChar, 15, NN = true)]
        public string Ip { get; set; }

        [Field(Field.Etype.SmallInt, NN = true)]
        public int Port { get; set; }




    }
}
