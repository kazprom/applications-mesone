using System;
using System.Collections.Generic;
using System.Text;

namespace S7_DB_gate
{
    public class S7connection : IDisposable
    {


        #region VARIABLES

        private Dictionary<int, TagSettings> tags = new Dictionary<int, TagSettings>();
        private Lib.Buffer<DB_gate_Lib.TagData> buffer;
        private Diagnostics diagnostics;

        #endregion


        #region PROPERTIES

        private long id;
        public long ID { get { return id; } }

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

        public S7connection(long id, Lib.Buffer<DB_gate_Lib.TagData> buffer, Diagnostics diagnostics)
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

        public void Settings(string ip, short port, short rack, short slot)
        {

            if (!ip.Equals(this.ip) || port != this.port || rack != this.rack || slot != this.slot)
            {

                this.ip = ip;
                this.port = port;
                this.rack = rack;
                this.slot = slot;


                Console.WriteLine($"Connect to {ip}:{port} rack:{rack} slot:{slot}");

            }
        }

        public void PutTag(int id, TagSettings tag)
        {
            try
            {
                lock (tags)
                {
                    if (!tags.ContainsKey(id))
                    {
                        tags.Add(id, new TagSettings());
                        Console.WriteLine($"Add tag DB{tag.db_no}.{tag.db_offset} [{tag.req_type}]");
                    }
                    tags[id] = tag;
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
                lock (tags)
                {
                    if (tags.ContainsKey(id))
                    {
                        TagSettings tag = tags[id];
                        Console.WriteLine($"Add tag DB{tag.db_no}.{tag.db_offset} [{tag.req_type}]");
                        tags.Remove(id);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error remove tag", ex);
            }
        }

        #endregion


    }
}
