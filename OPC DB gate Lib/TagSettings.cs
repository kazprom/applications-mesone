using System;
using System.Collections.Generic;
using System.Text;

namespace LibOPCDBgate
{
    [Serializable]
    public class TagSettings
    {

        public string path;
        public int rate;
        public LibDBgate.TagData.EDataType data_type;

        public bool rt_values_enabled;
        public bool history_enabled;

    }





}
