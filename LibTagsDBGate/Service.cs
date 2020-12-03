using LibPlcDBgate.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibPlcDBgate
{
    public class Service : LibDBgate.CSrvCUSTOM
    {


        #region CONSTANTS

        private const string depth_history_hour_name = "DEPTH_HISTORY_HOUR";
        private const uint depth_history_hour_default = 24;

        #endregion

        #region VARIABLES

        private DateTime his_ts = default;
        private Timer timer_DB_W;

        

        public Lib.Buffer<RetroValue> retro_buf;

        #endregion

        #region CONSTRUCTOR
        public Service(LibMESone.CSrvCore parent, ulong id) : base(parent, id)
        {

            retro_buf = new Lib.Buffer<RetroValue>(1000, 5000);
            retro_buf.CyclicEvent += RetroDataHandler;
            retro_buf.HalfEvent += RetroDataHandler;

            timer_DB_W = new Timer(DB_W_Handler, null, 0, 5000);

        }


        #endregion

        #region DESTRUCTOR

        public override void Dispose(bool disposing)
        {

            if (disposing)
            {

                WaitHandle h = new AutoResetEvent(false);
                timer_DB_W.Dispose(h);
                h.WaitOne();

                

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

        private void DB_W_Handler(object state)
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

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Title}. DB W handler");
            }
        }

        #endregion
        public override void DB_Handler(object state)
        {
            base.DB_Handler(state);

            try
            {
                if (Database != null)
                {

                    //------------retro cleaner-----------

                    uint depth_history_hours = depth_history_hour_default;
                    if (Settings.ContainsKey(depth_history_hour_name))
                    {
                        if (!uint.TryParse(Settings[depth_history_hour_name], out depth_history_hours))
                        {
                            logger.Warn($"Setting [{depth_history_hour_name}] can't parse. Default value is {depth_history_hour_default}");
                        }
                    }
                    else
                    {
                        logger.Warn($"Setting [{depth_history_hour_name}] not found. Default value is {depth_history_hour_default}");
                    }

                    IEnumerable<string> tables = Database.GetListTables(Structs.RetroValue.TablePrefix + "%");
                    DateTime ts = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);

                    foreach (string table in tables)
                    {
                        if (ts.Subtract(Structs.RetroValue.GetTimeStamp(table)).TotalHours > depth_history_hours)
                        {
                            if (Database.RemoveTable(table))
                                logger.Info($"{Title}. Removed retro table [{table}]");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Title}. DB handler");
            }

        }

    }
}
