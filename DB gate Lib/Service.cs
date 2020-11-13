using LibDBgate.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibDBgate
{
    public class Service : LibMESone.Service
    {

        #region VARIABLES

        private DateTime his_ts = default;
        private Timer timer_database_write;

        public Dictionary<ulong, Client> Clients = new Dictionary<ulong, Client>();

        public Lib.Buffer<RetroValue> retro_buf;

        #endregion

        #region CONSTRUCTOR
        public Service(LibMESone.Core parent, ulong id) : base(parent, id)
        {

            retro_buf = new Lib.Buffer<RetroValue>(1000, 5000);
            retro_buf.CyclicEvent += RetroDataHandler;
            retro_buf.HalfEvent += RetroDataHandler;

            timer_database_write = new Timer(DatabaseWriteHandler, null, 0, 5000);

        }


        #endregion

        #region DESTRUCTOR

        public override void Dispose(bool disposing)
        {

            if (disposing)
            {

                WaitHandle h = new AutoResetEvent(false);
                timer_database_write.Dispose(h);
                h.WaitOne();

                foreach (Client client in Clients.Values)
                {
                    client.Dispose();
                }

                Clients.Clear();

            }

            base.Dispose(disposing);

        }

        #endregion

        #region PRIVATES

        private async void RetroDataHandler()
        {
            await Task.Run(() =>
            {

                try
                {

                    RetroValue data = null;

                    if (Database != null)
                    {
                        while (retro_buf.Count > 0)
                        {

                            if (data == null)
                            {
                                data = retro_buf.Dequeue();


                                if (his_ts == default || his_ts != data.Timestamp)
                                {
                                    if (!Database.CheckExistTable(RetroValue.GetTableName(data.Timestamp)))
                                    {
                                        if (Database.CreateTable<RetroValue>(RetroValue.GetTableName(data.Timestamp)))
                                        {
                                            his_ts = data.Timestamp;
                                        }
                                    }
                                }

                                if (Database.Insert(RetroValue.GetTableName(data.Timestamp), data))
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
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"{Title}. Retro data handler");
                }
            });
        }

        private void DatabaseWriteHandler(object state)
        {
            try
            {
                if (Database != null)
                {

                    IEnumerable<Tag> rt_values = Clients.SelectMany(x => x.Value.Groups.SelectMany(y => y.Value.Tags.Values).Where(z => z.Timestamp != null));

                    foreach (Tag tag in rt_values)
                    {
                        Database.Update("rt_values", new RealTimeValue()
                        {
                            Tags_id = tag.ID,
                            Timestamp = (DateTime)tag.Timestamp,
                            Value_raw = Tag.ObjToBin(tag.Value),
                            Value_str = tag.Value != null ? tag.Value.ToString() : null,
                            Quality = (byte)tag.Quality
                        });
                    }

                    IEnumerable<Diagnostic> diagnostics = Clients.Select(x => x.Value.Diagnostic);

                    foreach (Diagnostic diagnostic in diagnostics)
                    {
                        Database.Update("diagnostics", diagnostic);
                    }

                    Database.WhereNotInDelete(Diagnostic.TableName,
                                              nameof(Diagnostic.Clients_id),
                                              Clients.Keys.ToArray());

                    Database.WhereNotInDelete(RealTimeValue.TableName,
                                              nameof(RealTimeValue.Tags_id),
                                              rt_values.Select(x => x.ID).ToArray());
                    //Clients.SelectMany(x => x.Value.Groups.SelectMany(y => y.Value.Tags.Select(z => z.Key))).ToArray());

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Title}. Actual data handler");
            }
        }

        #endregion

    }
}
