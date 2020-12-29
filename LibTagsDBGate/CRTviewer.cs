using LibMESone;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace LibPlcDBgate
{
    public class CRTviewer : CSrvDB
    {

        private List<Tables.CRtValue> TRTvalues { get; set; }

        public CRTviewer()
        {
            CycleRate = 5000;
        }


        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {
            try
            {
                lock (Children)
                {
                    foreach (CTag tag in Children.Values)
                    {
                        if (tag.RT_enabled)
                            TRTvalues.Add(new Tables.CRtValue()
                            {
                                Tags_id = tag.Id,
                                Timestamp = tag.Timestamp,
                                Quality = (byte)tag.Quality,
                                Value_str = tag.Value.ToString(),
                                Value_raw = CTag.ObjToBin(tag.Value)
                            });
                    }

                    if(DB != null)
                    {
                        


                    }
                }
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
                    Children.Add(tag.Id, tag);
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
                    Children.Remove(tag.Id);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

    }
}
