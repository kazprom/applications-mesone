using System;
using System.Collections.Generic;
using System.Text;

namespace OPC_DB_gate_Lib
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






        public long id;
        public DateTime timestamp;
        public object value;
        public EQuality quality;

        public static byte[] ObjToBin(object obj)
        {
            byte[] result = new byte[8];

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
                throw new Exception("Don't know data type of object");
            }

            return result;
        }






    }
}
