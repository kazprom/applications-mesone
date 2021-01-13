using System;
using System.Collections.Generic;
using System.Text;

namespace CSV_DB_gate
{
    public class CField
    {

        public string NameSource { get; set; }

        public string NameDestination { get; set; }

        public Lib.Field.EDoctrine? DataType { get; set; }

        public bool Unique { get; set; }

    }
}
