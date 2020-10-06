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


        public static object ObjToDataType(object obj, EDataType type)
        {
            try
            {
                if (obj == null)
                    return null;

                switch (type)
                {
                    case EDataType.dt_boolean:
                        return Convert.ToBoolean(obj);
                    case EDataType.dt_byte:
                        return Convert.ToByte(obj);
                    case EDataType.dt_char:
                        return Convert.ToChar(obj);
                    case EDataType.dt_double:
                        return Convert.ToDouble(obj);
                    case EDataType.dt_int16:
                        return Convert.ToInt16(obj);
                    case EDataType.dt_int32:
                        return Convert.ToInt32(obj);
                    case EDataType.dt_int64:
                        return Convert.ToInt64(obj);
                    case EDataType.dt_uint16:
                        return Convert.ToUInt16(obj);
                    case EDataType.dt_uint32:
                        return Convert.ToUInt32(obj);
                    case EDataType.dt_uint64:
                        return Convert.ToUInt64(obj);
                    default:
                        throw new Exception("Don't know data type.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error convert object to data type", ex);
            }

        }


    }





}
