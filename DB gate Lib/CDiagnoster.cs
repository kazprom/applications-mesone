using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace LibDBgate
{
    public class CDiagnoster : LibMESone.CSrvDB
    {


        public List<Tables.CDiagnostic> TDiagnostics { get; set; } = new List<Tables.CDiagnostic>();

        public CDiagnoster()
        {
            CycleRate = 5000;
        }

        public void Subcribe(CSUB sub)
        {
            try
            {
                lock (Children)
                {
                    Children.Add(sub.Id, sub);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public void Unsubcribe(CSUB sub)
        {
            try
            {
                lock (Children)
                {
                    Children.Remove(sub.Id);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }


        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {

            try
            {
                lock (Children)
                {
                    foreach (CSUB sub in Children.Values)
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

    }
}
