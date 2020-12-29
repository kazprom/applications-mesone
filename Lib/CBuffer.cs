using NLog;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lib
{
    public class CBuffer<T>
    {
        #region VARIABLES

        private Queue<T> q = new Queue<T>();

        private uint limit;

        #endregion

        #region EVENTS


        public delegate void EnqueueNotify(T obj);  // delegate
        public event EnqueueNotify EnqueueEvent; // event

        public delegate void DequeueNotify(T obj);  // delegate
        public event DequeueNotify DequeueEvent; // event

        public delegate void FullNotify(T obj);  // delegate
        public event FullNotify FullEvent; // event

        #endregion

        #region PROPERTIES

        public int Count { get { return q.Count; } }

        #endregion

        #region CONSTRUCTOR

        public CBuffer(uint limit)
        {
            this.limit = limit;
        }

        #endregion

        #region PUBLICS

        public void Enqueue(T obj)
        {

            q.Enqueue(obj);
            EnqueueEvent?.Invoke(obj);

            if (q.Count > limit)
                FullEvent?.Invoke(obj);

            while (q.Count > limit)
            {
                q.Dequeue();
            }
        }

        public T Dequeue()
        {
            T obj = q.Dequeue();
            DequeueEvent?.Invoke(obj);
            return obj;
        }

        #endregion



    }
}
