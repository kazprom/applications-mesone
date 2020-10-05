using System;
using System.Collections.Generic;
using System.Text;

namespace OPC_DB_gate_Lib
{
    [Serializable]
    public class TagSettings
    {

        [Serializable]
        public enum EDataType : byte
        {
            dt_boolean = 1,
            dt_byte = 2,
            dt_char = 3,
            dt_double = 4,
            dt_int16 = 5,
            dt_int32 = 6,
            dt_int64 = 7,
            dt_uint16 = 8,
            dt_uint32 = 9,
            dt_uint64 = 10
        }

        public string path;
        public int rate;
        public EDataType data_type;

    }



}
