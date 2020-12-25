using System;
using System.Collections.Generic;
using System.Text;

namespace LibPlcDBgate.Structs
{
    public class CClient : LibDBgate.Structs.CSetSUB
    {

        public string Ip { get; set; }

        public uint Port { get; set; }

        public IEnumerable<CGroup> Groups { get; set; }


    }
}
