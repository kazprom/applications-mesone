using System;
using System.Collections.Generic;
using System.Text;

namespace OPC_DB_gate_Lib
{
    [Serializable]
    public class TagSettings
    {

        public string path;
        public int rate;
        public DB_gate_Lib.TagData.EDataType data_type;

    }





}
