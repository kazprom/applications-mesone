using LibDBgate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace S7_DB_gate
{
    public class S7connection //: IDisposable
    {
        /*

        #region VARIABLES
        private string title = "";
        private Dictionary<int, Dictionary<int, TagSettings>> tag_groups = new Dictionary<int, Dictionary<int, TagSettings>>();
        private Dictionary<int, Timer> timers = new Dictionary<int, Timer>();
        private Lib.Buffer<LibDBgate.TagData> buffer;
        private Tables.Tdiagnostics diagnostics;

        private S7.Net.Plc plc;
        private Timer connection_handler;

        #endregion


        #region PROPERTIES

        private ClientSettings settings;
        public ClientSettings Settings
        {
            get
            {
                return settings;
            }
            set
            {

                if (settings == null ||
                    !value.cpu_type.Equals(settings.cpu_type) ||
                    !value.ip.Equals(settings.ip) ||
                    value.port != settings.port ||
                    value.rack != settings.rack ||
                    value.slot != settings.slot)
                {
                    settings = value;

                    if (plc != null && plc.IsConnected)
                    {
                        plc.Close();
                    }
                }

                title = $"name<{settings.name}> cpu[{settings.cpu_type}] ip {settings.ip}:{settings.port} [r{settings.rack}:s{settings.slot}]";

            }
        }



        private string state;
        public string State
        {
            get { return state; }
            private set
            {
                state = value;
                diagnostics.PutState(settings.id, state);
            }
        }

        #endregion


        #region CONSTRUCTOR

        public S7connection(ClientSettings settings, Lib.Buffer<LibDBgate.TagData> buffer, Tables.Tdiagnostics diagnostics)
        {
            try
            {
                Settings = settings;
                this.buffer = buffer;
                this.diagnostics = diagnostics;
                connection_handler = new Timer(ConnectionHandler, null, 0, 10000);

                Lib.Message.Make($"Created connection {title}");
            }
            catch (Exception ex)
            {
                throw new Exception("Error constructor", ex);
            }
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

                    int[][] tag_ids = tag_groups.Values.Select(x => x.Values.Select(y => y.id).ToArray()).ToArray();

                    foreach (var group in tag_ids)
                    {
                        foreach (var id in group)
                        {
                            RemoveTag(id);
                        }
                    }

                    if (plc != null && plc.IsConnected)
                    {
                        plc.Close();
                    }

                    Lib.Message.Make($"Removed connection {title}");

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


        public void PutTag(TagSettings tag)
        {
            try
            {
                lock (tag_groups)
                {
                    if (!tag_groups.ContainsKey(tag.rate))
                    {
                        tag_groups.Add(tag.rate, new Dictionary<int, TagSettings>());
                        timers.Add(tag.rate, new Timer(DataReader, tag_groups[tag.rate], 0, tag.rate));
                        Lib.Message.Make($"Add group [{tag.rate}]");
                    }

                    Dictionary<int, TagSettings> tags = tag_groups[tag.rate];

                    lock (tags)
                    {
                        if (!tags.ContainsKey(tag.id))
                        {
                            tags.Add(tag.id, new TagSettings());
                            Lib.Message.Make($"Add tag [{tag.name}] DB{tag.db_no}.{tag.db_offset} [{tag.req_type}]");
                        }

                        tags[tag.id] = tag;
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

        private void ConnectionHandler(object state)
        {

            try
            {
                if (settings != null && plc == null)
                {
                    plc = new S7.Net.Plc(settings.cpu_type, settings.ip, settings.port, settings.rack, settings.slot);
                }

                if (plc != null && !plc.IsConnected)
                {
                    plc.Open();
                    Lib.Message.Make($"Connect to {title}");
                    State = "Connected";
                }
            }
            catch (Exception ex)
            {
                Lib.Message.Make($"Can't connect to {title}", ex);
                State = "Disconnected";
            }

        }

        private void DataReader(object state)
        {
            Dictionary<int, TagSettings> group = state as Dictionary<int, TagSettings>;

            try
            {
                lock (group)
                {
                    foreach (var item in group)
                    {
                        TagSettings tag = item.Value;
                        try
                        {
                            if (tag.enabled && plc != null && plc.IsConnected)
                            {
                                object result = plc.Read(tag.plc_data_type, tag.db_no, tag.db_offset, tag.req_type, 1);

                                TagData data = new LibDBgate.TagData()
                                {
                                    id = item.Key,
                                    timestamp = DateTime.Now,
                                    quality = LibDBgate.TagData.EQuality.Good,
                                    value = LibDBgate.TagData.ObjToDataType(result, tag.data_type)
                                };

                                if (tag.rt_value_enabled)
                                    data.settings |= TagData.ESettings.rt_value_enabled;

                                if (tag.history_enabled)
                                    data.settings |= TagData.ESettings.history_enabled;

                                buffer.Enqueue(data);
                            }
                        }
                        catch (Exception ex)
                        {
                            Lib.Message.Make($"Error read tag DB{tag.db_no}.{tag.db_offset} [{tag.req_type}", ex);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                Lib.Message.Make($"Error read group", ex);
            }
        }

        #endregion
        */
    }
}
