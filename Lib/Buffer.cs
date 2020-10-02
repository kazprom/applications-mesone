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



        


        public int Count { get { return q.Count; } }

        private Queue<T> q = new Queue<T>();

        public Buffer(int limit)
        {
            this.limit = limit;
        }

        public void Enqueue(T obj)
        {

            q.Enqueue(obj);
            while (q.Count > limit)
            {
                q.Dequeue();
            }
        }

        public T Dequeue()
        {
            return q.Dequeue();
        }


    }
}
