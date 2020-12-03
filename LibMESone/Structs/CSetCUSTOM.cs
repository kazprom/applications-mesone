using System;
using System.Collections.Generic;
using System.Text;

namespace LibMESone.Structs
{
    public class CSetCUSTOM: CSetting
    {
        public string Name { get; set; }

        public string Database { get; set; }

        public string Driver { get; set; }

        public string Host { get; set; }

        public uint Port { get; set; }

        public string Charset { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }


    }
}
