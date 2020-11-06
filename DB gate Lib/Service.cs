using LibDBgate.Structs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LibDBgate
{
    public class Service : LibMESone.Service
    {

        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Lib.Buffer<Structs.RT_values> rt_buf;
        public Lib.Buffer<Structs.History> his_buf;

        public Service(LibMESone.Core parent, string name) : base(parent, name)
        {
            rt_buf = new Lib.Buffer<RT_values>(20, 1000);
            rt_buf.CyclicEvent += Rt_buf_Emptier;
            rt_buf.HalfEvent += Rt_buf_Emptier;

            his_buf = new Lib.Buffer<History>(1000, 5000);
            his_buf.CyclicEvent += His_buf_Emptier;
            his_buf.HalfEvent += His_buf_Emptier;
        }

        private void His_buf_Emptier()
        {
            try
            {

                History data = null;

                while (rt_buf.Count > 0)
                {
                    if (data == null)
                        data = his_buf.Dequeue();
                    if (database.Update(History.GetTableName(data.Timestamp), data))
                        data = null;
                    else
                        Thread.Sleep(1000);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{title}. Emptier history buffer");
            }
        }

        private void Rt_buf_Emptier()
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

        }



    }
}
