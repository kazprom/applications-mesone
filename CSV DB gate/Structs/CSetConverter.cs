using System;
using System.Collections.Generic;
using System.Text;

namespace CSV_DB_gate.Structs
{
    public class CSetConverter: LibDBgate.Structs.CSetSUB
    {

        public string Name { get; set; }

        public string File_path { get; set; }

        public bool File_delete { get; set; }

        public bool Table_clear { get; set; }

        public DateTime Start_timestamp { get; set; }

        public uint Frequency_sec { get; set; }

        public uint Timeout_sec { get; set; }

        public IEnumerable<CField> Fields { get; set; }



    }
}
