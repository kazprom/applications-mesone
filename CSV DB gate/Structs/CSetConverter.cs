using System;
using System.Collections.Generic;
using System.Text;

namespace CSV_DB_gate.Structs
{
    public class CSetConverter : LibDBgate.Structs.CSetSUB
    {

        public string Name { get; set; }

        public string Base_path { get; set; }

        public string File_path { get; set; }

        public bool File_delete { get; set; }

        public string His_path { get; set; }

        public uint File_depth_his { get; set; }

        public bool Table_clear { get; set; }

        public DateTime Start_timestamp { get; set; }

        public uint Frequency_sec { get; set; }

        public uint Timeout_sec { get; set; }

        public IEnumerable<CField> Fields { get; set; }

        public bool Has_header_record { get; set; }

        public string Delimiter { get; set; }

        public char Quote { get; set; }

        public bool Quotes_ignore { get; set; }

        public bool Detect_column_count_changes { get; set; }

        public bool Replaceable { get; set; }

    }
}
