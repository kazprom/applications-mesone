using LibMESone;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace LibPlcDBgate
{
    public class CRTviewer : CSrvDB
    {

        private const string title = "RTviewer";

        private List<CTag> tags = new List<CTag>();
        private List<Tables.CRtValue> TRTvalues = new List<Tables.CRtValue>();

        public override Logger Logger
        {
            set
            {
                base.Logger = LogManager.GetLogger($"{value.Name}.{title}");
            }
        }

        public CRTviewer()
        {
            CycleRate = 5000;
        }


        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {
            try
            {
                lock (tags)
                {
                    foreach (CTag tag in tags)
                    {
                        if (tag != null && tag.RT_enabled && tag.Timestamp != null)
                            TRTvalues.Add(new Tables.CRtValue()
                            {
                                Tags_id = tag.Id,
                                Timestamp = (DateTime)tag.Timestamp,
                                Quality = (byte)tag.Quality,
                                Value_str = tag.Value != null ? tag.Value.ToString() : null,
                                Value_raw = CTag.ObjToBin(tag.Value)
                            });
                    }

                }

                if (DB != null)
                {

                    foreach (var item in TRTvalues)
                    {
                        DB.Update<Tables.CRtValue>(Tables.CRtValue.TableName, item);
                    }

                    DB.WhereNotInDelete(Tables.CRtValue.TableName, nameof(Tables.CRtValue.Tags_id), TRTvalues.Select(x => x.Tags_id).ToArray());

                }

                TRTvalues.Clear();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            base.Timer_Handler(sender, e);
        }


        public void Subscribe(CTag tag)
        {
            try
            {
                lock (Children)
                {
                    tags.Add(tag);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public void Unsubscribe(CTag tag)
        {
            try
            {
                lock (Children)
                {
                    tags.Remove(tag);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

    }
}
