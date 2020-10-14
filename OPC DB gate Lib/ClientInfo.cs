using System;
using System.Collections.Generic;
using System.Text;

namespace LibOPCDBgate
{

    [Serializable]
    public class ClientInfo
    {
        public DateTime clock;
        public string appinfo;
        public string message;

    }
}
