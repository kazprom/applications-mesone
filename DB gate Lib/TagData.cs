using System;
using System.Collections.Generic;
using System.Text;

namespace LibDBgate
{
    [Serializable]
    public class TagData
    {

        [Serializable]
        public enum EQuality : byte
        {
            Bad = 0,
            Bad_Configuration_Error_in_Server = 4,
            Bad_Not_Connected = 8,
            Bad_Device_Failure = 12,
            Bad_Sensor_Failure = 16,
            Bad_Last_Know_Value_Passed = 20,
            Bad_Comm_Failure = 24,
            Bad_Out_of_Service = 28,
            Uncertain = 64,
            Uncertain_Last_Usable_Value_timeout_of_some_kind = 68,
            Uncertain_Sensor_not_Accurate_outside_of_limits = 80,
            Uncertain_Engineering_Units_exceeded = 84,
            Uncertain_Value_from_multiple_sources = 88,
            Good = 192,
            Good_Local_Override = 216
        }


        [Serializable]
        public enum EDataType : byte
        {
            Boolean = 1,
            Byte = 2,
            Char = 3,
            Double = 4,
            Int16 = 5,
            Int32 = 6,
            Int64 = 7,
            UInt16 = 8,
            UInt32 = 9,
            UInt64 = 10
        }


        [Serializable]
        [Flags]
        public enum ESettings : byte
        {
            rt_value_enabled = 1,
            history_enabled = 2
        }

        public long id;
        public DateTime timestamp;
        public object value;
        public EQuality quality;
        public ESettings settings;

        public static byte[] ObjToBin(object obj)
        {
            byte[] result = new byte[8];

            try
            {
                if (obj is Boolean)
                {
                    System.Buffer.BlockCopy(BitConverter.GetBytes((Boolean)obj), 0, result, 0, sizeof(Boolean));
                }
                else if (obj is Byte)
                {
                    System.Buffer.BlockCopy(BitConverter.GetBytes((Byte)obj), 0, result, 0, sizeof(Byte));
                }
                else if (obj is Char)
                {
                    System.Buffer.BlockCopy(BitConverter.GetBytes((Char)obj), 0, result, 0, sizeof(Char));
                }
                else if (obj is Double)
                {
                    System.Buffer.BlockCopy(BitConverter.GetBytes((Double)obj), 0, result, 0, sizeof(Double));
                }
                else if (obj is Int16)
                {
                    System.Buffer.BlockCopy(BitConverter.GetBytes((Int16)obj), 0, result, 0, sizeof(Int16));
                }
                else if (obj is Int32)
                {
                    System.Buffer.BlockCopy(BitConverter.GetBytes((Int32)obj), 0, result, 0, sizeof(Int32));
                }
                else if (obj is Int64)
                {
                    System.Buffer.BlockCopy(BitConverter.GetBytes((Int64)obj), 0, result, 0, sizeof(Int64));
                }
                else if (obj is UInt16)
                {
                    System.Buffer.BlockCopy(BitConverter.GetBytes((UInt16)obj), 0, result, 0, sizeof(UInt16));
                }
                else if (obj is UInt32)
                {
                    System.Buffer.BlockCopy(BitConverter.GetBytes((UInt32)obj), 0, result, 0, sizeof(UInt32));
                }
                else if (obj is UInt64)
                {
                    System.Buffer.BlockCopy(BitConverter.GetBytes((UInt64)obj), 0, result, 0, sizeof(UInt64));
                }
                else
                {
                    throw new Exception($"Don't know data type of object. Type of object is {obj.GetType()}");
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Error convert object to bin array", ex);
            }

            return result;
        }


        public static object ObjToDataType(object obj, EDataType type)
        {
            try
            {
                if (obj == null)
                    throw new Exception("Can't convert null object");

                switch (type)
                {
                    case EDataType.Boolean:
                        return Convert.ToBoolean(obj);
                    case EDataType.Byte:
                        return Convert.ToByte(obj);
                    case EDataType.Char:
                        return Convert.ToChar(obj);
                    case EDataType.Double:
                        return Convert.ToDouble(obj);
                    case EDataType.Int16:
                        return Convert.ToInt16(obj);
                    case EDataType.Int32:
                        return Convert.ToInt32(obj);
                    case EDataType.Int64:
                        return Convert.ToInt64(obj);
                    case EDataType.UInt16:
                        return Convert.ToUInt16(obj);
                    case EDataType.UInt32:
                        return Convert.ToUInt32(obj);
                    case EDataType.UInt64:
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

        public static Type DataTypeToType(EDataType type)
        {
            switch (type)
            {
                case EDataType.Boolean:
                    return typeof(bool);
                case EDataType.Byte:
                    return typeof(byte);
                case EDataType.Char:
                    return typeof(char);
                case EDataType.Double:
                    return typeof(double);
                case EDataType.Int16:
                    return typeof(Int16);
                case EDataType.Int32:
                    return typeof(Int32);
                case EDataType.Int64:
                    return typeof(Int64);
                case EDataType.UInt16:
                    return typeof(UInt16);
                case EDataType.UInt32:
                    return typeof(UInt32);
                case EDataType.UInt64:
                    return typeof(UInt64);
                default:
                    throw new Exception("Don't know data type");
            }
        }



    }
}
