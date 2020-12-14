using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LibMESone
{
    public abstract class CCyclycService : IParent, IChild
    {

        #region VARIABLES

        private Timer timer;

        #endregion

        #region PROPERTIES

        public Logger Logger { get; set; }
        public ulong ID { get; set; }
        public string Name { get; set; }
        public IParent Parent { get; set; }
        public Dictionary<ulong, IChild> Children { get; set; }

        private uint cycle_rate;
        public uint CycleRate
        {
            get { return cycle_rate; }
            set
            {
                if (cycle_rate != value)
                {
                    cycle_rate = value;

                    if (timer != null)
                    {
                        WaitHandle h = new AutoResetEvent(false);
                        timer.Dispose(h);
                        h.WaitOne();
                        timer = null;
                    }

                    if (cycle_rate > 0)
                    {
                        timer = new Timer(Timer_Handler, null, 0, (int)cycle_rate);
                    }
                }
            }
        }


        public abstract void Timer_Handler(object state);

        public abstract void LoadSetting(ISetting setting);

        #endregion

        #region CONSTRUCTOR

        public CCyclycService()
        {
            try
            {
                Children = new Dictionary<ulong, IChild>();
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region DESTRUCTOR

        protected bool disposedValue;
        public virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CycleRate = 0;

                    foreach (IChild child in Children.Values)
                    {
                        child.Dispose();
                    }

                    Children.Clear();

                }

                disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);

            if (Logger != null) Logger.Info($"Disposed");

        }
        #endregion

        #region PUBLICS

        public void CUD<T>(IEnumerable<CSetting> settings) where T : IChild, new()
        {
            try
            {
                IEnumerable<ulong> fresh_ids = settings.Select(x => (ulong)x.Id);
                IEnumerable<ulong> existing_ids = Children.Keys;

                IEnumerable<ulong> waste = existing_ids.Except(fresh_ids);
                IEnumerable<ulong> modify = fresh_ids.Intersect(existing_ids);
                IEnumerable<ulong> missing = fresh_ids.Except(existing_ids);

                foreach (ulong id in waste)
                {
                    Children[id].Dispose();
                    Children.Remove(id);
                }

                foreach (ulong id in modify)
                {
                    CSetting setting = settings.First(x => x.Id == id);

                    Children[id].LoadSetting(setting);
                }

                foreach (ulong id in missing)
                {
                    CSetting setting = settings.First(x => x.Id == id);
                    IChild instance = new T
                    {
                        Parent = this,
                        ID = setting.Id,
                        Name = setting.Name
                    };
                    instance.LoadSetting(setting);
                    Children.Add(id, instance);
                }
            }
            catch (Exception ex)
            {
                if (Logger != null) Logger.Error(ex);
            }
        }

        #endregion

    }
}
