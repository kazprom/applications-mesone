using System;
using System.Collections.Generic;
using System.Text;

namespace S7_DB_gate.Structs
{
    public class CTag: LibPlcDBgate.Structs.CTag
    {

        public string PLC_data_type { get; set; }

        public int Data_block_no { get; set; }

        public int Data_block_offset { get; set; }

        public byte Bit_offset { get; set; }

        public string Request_type { get; set; }


    }
}
