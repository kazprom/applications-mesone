﻿using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;
using System.Xml;

namespace LibMESone.Structs
{
    public class BaseNE : BaseID
    {
        [Field(TYPE = Field.Etype.VarChar, SIZE = 255)]
        public string Name { get; set; }

        [Field(TYPE = Field.Etype.TinyInt,SIZE = 1)]
        public bool Enabled { get; set; }

     
    }
}
