using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMESone.Structs
{
    public class ServiceTypes : BaseID
    {
        [Field(TYPE = Field.Etype.VarChar, SIZE = 255, UQ = true)]
        public string Name { get; set; }

        [Field(TYPE = Field.Etype.VarChar, SIZE = 255)]
        public string Guid { get; set; }
    
    }
}
