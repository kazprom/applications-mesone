using System;
using System.Collections.Generic;
using System.Text;

namespace Lib
{
    public class Buffer<T>
    {
        #region CONSTANTS

        private int limit;

        #endregion


        #region EVENTS

        #region EVENTS

        public delegate void EnqueueNotify(T obj);  // delegate
        public event EnqueueNotify EnqueueEvent; // event

        public delegate void DequeueNotify(T obj);  // delegate
        public event DequeueNotify DequeueEvent; // event

        public delegate void HalfNotify();  // delegate
        public event HalfNotify HalfEvent; // event


        #endregion

        #endregion



        public int Count { get { return q.Count; } }

        private Queue<T> q = new Queue<T>();

        public Buffer(int limit)
        {
            this.limit = limit;
        }

        public void Enqueue(T obj)
        {

            q.Enqueue(obj);
            EnqueueEvent?.Invoke(obj);

            if (q.Count > limit / 2)
                HalfEvent?.Invoke();

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


    }
}
