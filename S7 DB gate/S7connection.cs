using LibDBgate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace S7_DB_gate
{
    public class S7connection : IDisposable
    {


        #region VARIABLES

        private Dictionary<int, Dictionary<int, TagSettings>> tag_groups = new Dictionary<int, Dictionary<int, TagSettings>>();
        private Dictionary<int, Timer> timers = new Dictionary<int, Timer>();
        private Lib.Buffer<LibDBgate.TagData> buffer;
        private Diagnostics diagnostics;

        S7.Net.Plc plc;

        #endregion


        #region PROPERTIES

        private long id;
        public long ID { get { return id; } }

        private S7.Net.CpuType cpu_type;
        public S7.Net.CpuType CPUtype { get { return cpu_type; } }

        private string ip;
        public string IP { get { return ip; } }

        private short port;
        public short Port { get { return port; } }

        private short rack;
        public short Rack { get { return rack; } }

        private short slot;
        public short Slot { get { return slot; } }

        private string state;
        public string State
        {
            get { return state; }
            private set
            {
                state = value;
                diagnostics.PutState(id, state);
            }
        }

        #endregion


        #region CONSTRUCTOR

        public S7connection(long id, Lib.Buffer<LibDBgate.TagData> buffer, Diagnostics diagnostics)
        {
            this.id = id;
            this.buffer = buffer;
            this.diagnostics = diagnostics;
        }

        #endregion


        #region DESTRUCTOR

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты)
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                // TODO: установить значение NULL для больших полей
                disposedValue = true;
            }
        }

        // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        // ~S7connection()
        // {
        //     // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion



        #region PUBLICS

        public void Settings(S7.Net.CpuType cpu_type, string ip, short port, short rack, short slot)
        {
            try
            {
                if (!cpu_type.Equals(this.cpu_type) || !ip.Equals(this.ip) || port != this.port || rack != this.rack || slot != this.slot)
                {
                    if (plc == null && plc.IsConnected)
                    {
                        plc.Close();
                    }

                    this.cpu_type = cpu_type;
                    this.ip = ip;
                    this.port = port;
                    this.rack = rack;
                    this.slot = slot;

                    plc = new S7.Net.Plc(this.cpu_type, this.ip, this.rack, this.slot);
                    plc.Open();


                    Lib.Message.Make($"Connect to {ip}:{port} rack:{rack} slot:{slot}");

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error settings", ex);
            }

        }

        public void PutTag(int id, TagSettings tag)
        {
            try
            {
                lock (tag_groups)
                {
                    if (!tag_groups.ContainsKey(tag.rate))
                    {
                        tag_groups.Add(tag.rate, new Dictionary<int, TagSettings>());
                        timers.Add(tag.rate, new Timer(TimerCallback, tag_groups[tag.rate], 0, tag.rate));
                        Lib.Message.Make($"Add group {tag.rate}");
                    }

                    Dictionary<int, TagSettings> tags = tag_groups[tag.rate];

                    lock (tags)
                    {
                        if (!tags.ContainsKey(id))
                        {
                            tags.Add(id, new TagSettings());
                            Lib.Message.Make($"Add tag DB{tag.db_no}.{tag.db_offset} [{tag.req_type}]");
                        }

                        tags[id] = tag;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error put tag", ex);
            }
        }

        public void RemoveTag(int id)
        {
            try
            {
                lock (tag_groups)
                {
                    List<int> unnecessary = new List<int>();

                    foreach (var group in tag_groups)
                    {

                        Dictionary<int, TagSettings> tags = group.Value;

                        if (tags.ContainsKey(id))
                        {
                            TagSettings tag = tags[id];
                            Lib.Message.Make($"Remove tag DB{tag.db_no}.{tag.db_offset} [{tag.req_type}]");
                            tags.Remove(id);
                        }

                        if (tags.Count == 0)
                            unnecessary.Add(group.Key);
                    }

                    foreach (var key in unnecessary)
                    {
                        tag_groups.Remove(key);
                        timers[key].Dispose();
                        timers.Remove(key);
                        Lib.Message.Make($"Remove group {key}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error remove tag", ex);
            }
        }

        #endregion


        #region PRIVATES

        private void TimerCallback(object state)
        {
            Dictionary<int, TagSettings> group = state as Dictionary<int, TagSettings>;

            try
            {
                foreach (var item in group)
                {
                    TagSettings tag = item.Value;
                    try
                    {
                        if (plc != null && plc.IsConnected)
                        {
                            object result = plc.Read(tag.plc_data_type, tag.db_no, tag.db_offset, tag.req_type, 1);
                            buffer.Enqueue(new LibDBgate.TagData()
                            {
                                id = item.Key,
                                timestamp = DateTime.Now,
                                quality = LibDBgate.TagData.EQuality.Good,
                                value = result
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Lib.Message.Make($"Error read tag DB{tag.db_no}.{tag.db_offset} [{tag.req_type}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Lib.Message.Make($"Error read group", ex);
            }
        }

        #endregion

    }
}
