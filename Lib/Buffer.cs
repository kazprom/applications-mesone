using NLog;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Lib
{
    public class Buffer<T>
    {
        #region VARIABLES

        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private Queue<T> q = new Queue<T>();

        private uint limit;
        private uint time_storage;
        

        private Timer timer;

        #endregion

        #region EVENTS


        public delegate void EnqueueNotify(T obj);  // delegate
        public event EnqueueNotify EnqueueEvent; // event

        public delegate void DequeueNotify(T obj);  // delegate
        public event DequeueNotify DequeueEvent; // event

        public delegate void HalfNotify();  // delegate
        public event HalfNotify HalfEvent; // event

        public delegate void CyclicNotify();  // delegate
        public event CyclicNotify CyclicEvent; // event

        #endregion

        #region PROPERTIES

        public int Count { get { return q.Count; } }

        #endregion



        public Buffer(uint limit, 
                      uint time_storage)
        {
            this.limit = limit;
            this.time_storage = time_storage;

            timer = new Timer(Cleaner, null, 0, this.time_storage);
        }

        public void Enqueue(T obj)
        {

            q.Enqueue(obj);
            EnqueueEvent?.Invoke(obj);

            if (q.Count > limit / 2)
            {
                HalfEvent?.Invoke();
            }

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


        #region PRIVATES

        private void Cleaner(object state)
        {

            CyclicEvent?.Invoke();

            try
            {

                

                

            }
            catch (Exception ex)
            {
                logger.Error(ex, "buffer cleaner");
            }
        }

        #endregion

    }
}
