using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMESone.Tables.Core
{
    public class CServiceType : CBaseID
    {
        static public string TableName = "service_types";

        [Field(TYPE = Field.EDoctrine.String, SIZE = 255, UQ = true, NN = true)]
        public string Name { get; set; }

        [Field(TYPE = Field.EDoctrine.String, SIZE = 40, UQ = true, NN = true)]
        public string Guid { get; set; }
    
    }
}
