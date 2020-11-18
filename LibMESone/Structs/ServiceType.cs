using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMESone.Structs
{
    public class ServiceType : BaseID
    {
        [Field(TYPE = Field.Etype.VarChar, SIZE = 255, UQ = true, NN = true)]
        public string Name { get; set; }

        [Field(TYPE = Field.Etype.VarChar, SIZE = 40, UQ = true, NN = true)]
        public string Guid { get; set; }
    
    }
}
