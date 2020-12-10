using System;
using System.Collections.Generic;
using System.Text;

namespace CSV_DB_gate.Structs
{
    public class CField
    {

        public string NameSource { get; set; }

        public string NameDestination { get; set; }

        public Lib.Field.EDoctrine? DataType { get; set; }

    }
}
