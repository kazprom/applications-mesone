using LibDBgate.Structs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibDBgate
{
    public class Service : LibMESone.Service
    {

        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private DateTime his_ts = default;

        public Lib.Buffer<Structs.RT_values> rt_buf;
        public Lib.Buffer<Structs.History> his_buf;

        public Service(LibMESone.Core parent, string name) : base(parent, name)
        {
            rt_buf = new Lib.Buffer<RT_values>(20, 1000);
            rt_buf.CyclicEvent += Rt_buf_EmptierAsync;
            rt_buf.HalfEvent += Rt_buf_EmptierAsync;

            his_buf = new Lib.Buffer<History>(1000, 5000);
            his_buf.CyclicEvent += His_buf_Emptier;
            his_buf.HalfEvent += His_buf_Emptier;
        }

        private async void His_buf_Emptier()
        {
            await Task.Run(() =>
            {

                try
                {

                    History data = null;

                    while (his_buf.Count > 0)
                    {

                        if (data == null)
                        {
                            data = his_buf.Dequeue();


                            if (his_ts == default || his_ts != data.Timestamp)
                            {
                                if (!database.CheckExistTable(History.GetTableName(data.Timestamp)))
                                {
                                    if (database.CreateTable<History>(History.GetTableName(data.Timestamp)))
                                    {
                                        his_ts = data.Timestamp;
                                    }
                                }
                            }

                            if (database.Insert(History.GetTableName(data.Timestamp), data))
                            {
                                data = null;
                            }
                            else
                            {
                                Thread.Sleep(1000);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"{title}. Emptier history buffer");
                }
            });
        }

        private async void Rt_buf_EmptierAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    RT_values data = null;

                    while (rt_buf.Count > 0)
                    {
                        if (data == null)
                            data = rt_buf.Dequeue();
                        if (database.Update("rt_values", data))
                            data = null;
                        else
                            Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"{title}. Emptier realtime buffer");
                }
            });

        }



    }
}
