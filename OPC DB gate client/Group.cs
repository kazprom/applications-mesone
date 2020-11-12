using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPC_DB_gate_client
{
    class Group : IDisposable
    {
        #region STRUCTURES
        public struct STag
        {
            public long id;
            public string path;
            public LibDBgate.Tag.EDataType data_type;
        }

        #endregion

        #region VARIABLES

        public List<STag> tags = new List<STag>();

        #endregion

        #region PROPERTIES

        public bool IsDisposed { get { return disposedValue; } }


        private int rate;
        public int Rate { get { return rate; } }

        #endregion

        #region CONSTRUCTOR

        public Group(int rate)
        {
            this.rate = rate;
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
        // ~Group()
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

        






    }
}
