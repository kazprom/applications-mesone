using Lib;
using LibMESone;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibPlcDBgate
{
    [Serializable]
    public class CTag : Lib.CChild
    {


        #region ENUMS

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

        #endregion

        #region PROPERTIES

        public override CParent Parent
        {
            get => base.Parent;
            set
            {


                try
                {

                    if (!Equals(base.Parent, value))
                    {

                        base.Parent = value;
                        Lib.CParent parent = this.Parent;
                        while (parent != null)
                        {
                            if (parent is CCUSTOM)
                            {
                                CCUSTOM custom = parent as CCUSTOM;
                                Historian = custom.Historian;
                                custom.RTviewer.Subscribe(this);
                            }

                            parent = parent.Parent;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

            }
        }


        public CHistorian Historian { get; set; }

        private EDataType data_type;
        public dynamic Data_type
        {
            get { return data_type; }
            set
            {
                try
                {
                    data_type = Enum.Parse(typeof(EDataType), Convert.ToString(value), true);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
            }
        }

        private bool? rt_enabled;
        public dynamic RT_enabled
        {
            get { return rt_enabled; }
            set
            {
                try
                {
                    rt_enabled = bool.Parse(Convert.ToString(value));
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
            }
        }

        private bool? history_enabled;
        public dynamic History_enabled
        {
            get { return history_enabled; }
            set
            {
                try
                {
                    history_enabled = bool.Parse(Convert.ToString(value));
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
            }
        }

        public DateTime Timestamp { get; set; }

        private object value;
        public object Value
        {
            get { return value; }
            set
            {
                try
                {
                    this.value = ObjToDataType(value, data_type);

                    if (history_enabled == true)
                    {
                        if (Historian != null) Historian.Put(Id, Timestamp, ObjToBin(Value), (byte)Quality);
                    }

                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
            }
        }

        public EQuality Quality { get; set; }

        #endregion


        private static object ObjToDataType(object obj, EDataType type)
        {
            try
            {
                if (obj == null)
                    return null;

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
                }

                return null;

            }
            catch (Exception ex)
            {
                throw new Exception("Error convert object to data type", ex);
            }

        }
        public static byte[] ObjToBin(object obj)
        {
            byte[] result = new byte[8];

            try
            {
                if (obj is Boolean)
                {
                    System.Buffer.BlockCopy(BitConverter.GetBytes((Boolean)obj), 0, result, 0, sizeof(Boolean));
                }
                else if (obj is Byte @byte)
                {
                    System.Buffer.BlockCopy(BitConverter.GetBytes(@byte), 0, result, 0, sizeof(Byte));
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
                else if (obj == null)
                {
                    return null;
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







        private static Type DataTypeToType(EDataType type)
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
