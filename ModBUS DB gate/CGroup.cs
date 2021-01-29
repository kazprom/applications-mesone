using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace ModBUS_DB_gate
{
    class CGroup : LibPlcDBgate.CGroup
    {

        public override dynamic Tags
        {
            set
            {
                try
                {

                    var data = from tags in (IEnumerable<dynamic>)value
                               select new
                               {

                                   Parent = this,
                                   tags.Id,
                                   tags.Name,

                                   tags.Data_type,
                                   tags.History_enabled,
                                   RT_enabled = tags.RT_values_enabled,

                                   tags.Address,
                                   tags.Function,
                               };



                    Dictionary<ulong, Dictionary<string, object>> children_props = data.ToDictionary(o => (ulong)o.Id,
                                                                                                     o => o.
                                                                                                          GetType().
                                                                                                          GetProperties().ToDictionary(z => z.Name,
                                                                                                                                       z => z.GetValue(o)));

                    CUD<CTag>(children_props);

                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

            }
        }


        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {

            try
            {

                lock (Children)
                {

                    foreach (CTag tag in Children.Values)
                    {
                        if (tag != null && (tag.RT_enabled || tag.History_enabled))
                        {

                            //tag.Timestamp = DateTime.Now;
                            CClient parent = (CClient)Parent;

                            byte l = LibPlcDBgate.CTag.SizeOfDataType(tag.Data_type);


                            if (parent.Plc != null)
                            {


                                bool[] bool_arr = { false };
                                ushort[] ushort_arr = { 0 };
                                byte[] byte_arr = { 0, 0, 0, 0, 0, 0, 0, 0 };

                                try
                                {

                                    switch (tag.Function)
                                    {
                                        case CTag.EFunctions.CoilStatus: bool_arr = parent.Plc.ReadCoils(parent.Address, tag.Address, l); byte_arr[0] = Convert.ToByte(bool_arr[0]); break;
                                        case CTag.EFunctions.InputStatus: bool_arr = parent.Plc.ReadInputs(parent.Address, tag.Address, l); byte_arr[0] = Convert.ToByte(bool_arr[0]); break;
                                        case CTag.EFunctions.HoldingRegister: ushort_arr = parent.Plc.ReadHoldingRegisters(parent.Address, tag.Address, l); Buffer.BlockCopy(ushort_arr, 0, byte_arr, 0, ushort_arr.Length * 2); break;
                                        case CTag.EFunctions.InputRegister: ushort_arr = parent.Plc.ReadInputRegisters(parent.Address, tag.Address, l); Buffer.BlockCopy(ushort_arr, 0, byte_arr, 0, ushort_arr.Length * 2); break;
                                    }

                                }
                                catch (Exception ex)
                                {
                                    Logger.Warn(ex, $"Can't read tag {tag.Function} {tag.Address} {tag.Data_type}");
                                }


                                if (parent.Sb)
                                {
                                    for (int i = 0; i < byte_arr.Length; i += 2)
                                    {
                                        byte b = byte_arr[i];
                                        byte_arr[i] = byte_arr[i + 1];
                                        byte_arr[i + 1] = b;
                                    }
                                }

                                if (parent.Sw)
                                {
                                    for (int i = 0; i < byte_arr.Length; i += 4)
                                    {
                                        byte[] w = { 0, 0 };
                                        Buffer.BlockCopy(byte_arr, i, w, 0, 2);
                                        Buffer.BlockCopy(byte_arr, i + 2, byte_arr, i, 2);
                                        Buffer.BlockCopy(w, 0, byte_arr, i + 2, 2);
                                    }

                                }

                                object result = null;

                                switch (tag.Data_type)
                                {
                                    case LibPlcDBgate.CTag.EDataType.Boolean:
                                        result = BitConverter.ToBoolean(byte_arr);
                                        break;
                                    case LibPlcDBgate.CTag.EDataType.Byte:
                                        result = byte_arr[0];
                                        break;
                                    case LibPlcDBgate.CTag.EDataType.Char:
                                        result = BitConverter.ToChar(byte_arr);
                                        break;
                                    case LibPlcDBgate.CTag.EDataType.Int16:
                                        result = BitConverter.ToInt16(byte_arr);
                                        break;
                                    case LibPlcDBgate.CTag.EDataType.UInt16:
                                        result = BitConverter.ToUInt16(byte_arr);
                                        break;
                                    case LibPlcDBgate.CTag.EDataType.Int32:
                                        result = BitConverter.ToInt32(byte_arr);
                                        break;
                                    case LibPlcDBgate.CTag.EDataType.UInt32:
                                        result = BitConverter.ToUInt32(byte_arr);
                                        break;
                                    case LibPlcDBgate.CTag.EDataType.Double:
                                        result = BitConverter.ToDouble(byte_arr);
                                        break;
                                    case LibPlcDBgate.CTag.EDataType.Int64:
                                        result = BitConverter.ToInt64(byte_arr);
                                        break;
                                    case LibPlcDBgate.CTag.EDataType.UInt64:
                                        result = BitConverter.ToUInt64(byte_arr);
                                        break;
                                    case LibPlcDBgate.CTag.EDataType.Single:
                                        result = BitConverter.ToSingle(byte_arr);
                                        break;
                                }

                                tag.Quality = LibPlcDBgate.CTag.EQuality.Good;
                                tag.Value = result;
                            }
                            else
                            {
                                tag.Quality = LibPlcDBgate.CTag.EQuality.Bad_Comm_Failure;
                                tag.Value = null;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            base.Timer_Handler(sender, e);
        }

    }
}
