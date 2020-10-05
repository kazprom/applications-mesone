using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPC_DB_gate_client
{
    public class DataReaders
    {

        #region PROPERTIES

        private Dictionary<int, OPC_DB_gate_Lib.TagSettings> tags;
        public Dictionary<int, OPC_DB_gate_Lib.TagSettings> Tags { get { return tags; } }

        #endregion


        public DataReaders(Dictionary<int, OPC_DB_gate_Lib.TagSettings> tags)
        {

            this.tags = tags;

        }

    }
}
