using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Timers;

namespace KingPigeonS272_DB_gate
{
    class CClient : LibMESone.CSrvCyc
    {

        #region CONSTS

        const string AI_prefix = "AIN";
        const string DI_prefix = "DIN";
        const string Timestamp_key = "TS";
        const string Temperature_key = "Temp";
        const string Humidity_key = "Hum";


        const byte AI_channels = 6;
        const byte DI_channels = 8;

        #endregion

        #region VARIABLES

        private DateTime ts;

        #endregion


        #region PROPERTIES

        private string imei;
        public string Imei
        {
            get
            {
                return (imei);
            }
            set
            {
                if (!Equals(imei, value))
                {
                    imei = value;
                    Logger.Info($"IMEI = {imei}");
                }
            }
        }

        private ushort timeout_m;
        public ushort Timeout_m
        {
            get
            {
                return (timeout_m);
            }
            set
            {
                if (timeout_m != value)
                {
                    timeout_m = value;
                    Logger.Info($"Timeout = {timeout_m}");
                }
            }
        }

        public dynamic Tags
        {
            set
            {
                try
                {

                    var data = from tags in (IEnumerable<dynamic>)value
                               group tags by tags.Rate into groups
                               select new
                               {
                                   Parent = this,
                                   Id = groups.Key,

                                   Tags = from group_tags in groups
                                          select new
                                          {
                                              group_tags.Id,
                                              group_tags.Name,

                                              group_tags.Channel,

                                              group_tags.Data_type,
                                              group_tags.History_enabled,
                                              group_tags.RT_values_enabled

                                          }
                               };



                    Dictionary<ulong, Dictionary<string, object>> children_props = data.ToDictionary(o => (ulong)o.Id,
                                                                                                     o => o.
                                                                                                          GetType().
                                                                                                          GetProperties().ToDictionary(z => z.Name,
                                                                                                                                       z => z.GetValue(o)));
                    CUD<CGroup>(children_props);


                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }


        public Dictionary<string, object> Data { get; private set; } = new Dictionary<string, object>();

        #endregion



        public CClient()
        {
            try
            {
                Data.Add(Timestamp_key, null);

                for (int i = 0; i < AI_channels; i++)
                {
                    Data.Add($"{AI_prefix}{i}", null);
                }

                for (int i = 0; i < DI_channels; i++)
                {
                    Data.Add($"{DI_prefix}{i}", null);
                }

                Data.Add(Temperature_key, null);
                Data.Add(Humidity_key, null);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }


        public void LoadData(byte[] data)
        {
            try
            {
                List<byte> _data = new List<byte>();
                _data.AddRange(data);

                DateTime? ts = GetTimestamp(ref _data);

                if (ts != null)
                {

                    Data[Timestamp_key] = ts;

                    for (byte i = 0; i < AI_channels; i++)
                    {
                        Data[$"{AI_prefix}{i}"] = GetData<float>(2, i, ref _data);
                    }

                    for (byte i = 0; i < DI_channels; i++)
                    {
                        Data[$"{DI_prefix}{i}"] = GetData<bool>(3, i, ref _data);
                    }

                    for (byte i = 0; i < 4; i++)
                    {
                        GetData<float>(5, i, ref _data);
                    }

                    Data[Temperature_key] = GetData<float>(4, 0, ref _data);
                    Data[Humidity_key] = GetData<float>(4, 1, ref _data);


                }


            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private DateTime? GetTimestamp(ref List<byte> data)
        {
            try
            {

                const byte symbol = 0x7F;

                if (data != null &&
                    data.Count >= 8 &&
                    data[0] == symbol &&
                    data[1] == symbol)
                {
                    data.RemoveRange(0, 2);
                    DateTime result = new DateTime(2000 + data[0], data[1], data[2], data[3], data[4], data[5]);
                    data.RemoveRange(0, 6);
                    return result;
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }

        private object GetData<T>(byte prefix, byte channel, ref List<byte> data)
        {
            object result = null;
            try
            {
                if (data != null &&
                    data.Count >= 6 &&
                    data[0] == prefix &&
                    data[1] == channel)
                {

                    data.RemoveRange(0, 2);

                    TypeCode tc = Type.GetTypeCode(typeof(T));
                    switch (tc)
                    {
                        case TypeCode.Boolean:
                            result = BitConverter.ToInt32(data.GetRange(0, 4).ToArray()) != 0;
                            break;
                        case TypeCode.Single:
                            result = BitConverter.ToSingle(data.GetRange(0, 4).ToArray());
                            break;
                        default:
                            Logger.Warn($"Unknown data type {tc}");
                            break;
                    }

                    Logger.Debug($"{prefix}{channel} val = {result}");

                    data.RemoveRange(0, 4);

                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return result;

        }
    }
}
