using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPC_DB_gate_client
{
    class OPCclient:IDataReader
    {


        public OPCclient(string name, Lib.Buffer<OPC_DB_gate_Lib.TagData> buffer): base(name, buffer)
        {

        }

    }
}
