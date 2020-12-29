using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OPC_DB_gate_client
{
    abstract class IDataReader : IDisposable
    {

        #region STRUCTURES
        public struct STag
        {
            public int id;
            public string path;
            public int rate;
            public LibDBgate.Tag.EDataType data_type;
        }

        #endregion

        #region VARIABLES

        protected Dictionary<int, Group> groups = new Dictionary<int, Group>();
        protected Lib.CBuffer<LibDBgate.Tag> buffer;

        #endregion

        #region PROPERTIES

        protected string name;
        public string Name { get { return name; } }

        public bool IsDisposed { get { return disposedValue; } }

        #endregion

        #region EVENTS

        protected delegate void PutNotify(Dictionary<int, Group> groups);  // delegate
        protected event PutNotify PutEvent; // event

        #endregion

        #region CONSTRUCTOR

        public IDataReader(string name, Lib.CBuffer<LibDBgate.Tag> buffer)
        {
            this.name = name;
            this.buffer = buffer;

            Lib.Message.Make($"Run {name}");

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

                    Lib.Message.Make($"Stopped {name}");
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                // TODO: установить значение NULL для больших полей
                disposedValue = true;
            }
        }

        // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        // ~DataReader()
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

        public void Put(List<STag> tags)
        {
            try
            {
                Dictionary<int, List<Group.STag>> tag_groups = new Dictionary<int, List<Group.STag>>();


                foreach (var item in tags)
                {
                    if (!tag_groups.ContainsKey(item.rate))
                        tag_groups.Add(item.rate, new List<Group.STag>());
                    tag_groups[item.rate].Add(new Group.STag() { id = item.id, path = item.path, data_type = item.data_type });
                }

                lock (groups)
                {
                    foreach (var item in groups)
                    {
                        if (!tag_groups.ContainsKey(item.Key))
                            item.Value.Dispose();
                    }

                    var itemsToRemove = groups.Where(f => f.Value.IsDisposed == true).ToArray();
                    foreach (var item in itemsToRemove)
                        groups.Remove(item.Key);


                    foreach (var item in tag_groups)
                    {
                        if (!groups.ContainsKey(item.Key))
                            groups.Add(item.Key, new Group(item.Key));
                        groups[item.Key].tags = item.Value;
                    }
                }

                PutEvent?.Invoke(groups);

            }
            catch (Exception ex)
            {

                throw new Exception("Error group tags", ex);
            }
        }


        #endregion

    }
}
