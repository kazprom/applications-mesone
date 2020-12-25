using System;
using System.Collections.Generic;
using System.Text;

namespace S7_DB_gate.Structs
{
    public class CClient : LibPlcDBgate.Structs.CClient
    {

        public string Cpu_type { get; set; }

        public ushort Rack { get; set; }

        public ushort Slot { get; set; }

    }
}
