using System;
using System.Collections.Generic;
using System.Text;

namespace S7_DB_gate
{
    public class TagSettings
    {

        public S7.Net.DataType plc_data_type;
        public int db_no;
        public int db_offset;
        public int rate;
        public S7.Net.VarType req_type;
        public LibDBgate.TagData.EDataType data_type;
        public bool rt_value_enabled;
        public bool history_enabled;


    }





}
