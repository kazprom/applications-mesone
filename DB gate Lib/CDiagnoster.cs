using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace LibDBgate
{
    public class CDiagnoster : LibMESone.CSrvDB
    {

        private const string title = "Diagnoster";

        private List<CSUB> subs = new List<CSUB>();
        public List<Tables.CDiagnostic> TDiagnostics { get; set; } = new List<Tables.CDiagnostic>();

        public CDiagnoster()
        {
            CycleRate = 10000;
        }

        public void Subscribe(CSUB sub)
        {
            try
            {
                lock (subs)
                {
                    subs.Add(sub);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public void Unsubscribe(CSUB sub)
        {
            try
            {
                lock (subs)
                {
                    subs.Remove(sub);
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
                lock (subs)
                {
                    foreach (CSUB sub in subs)
                    {
                        TDiagnostics.Add(sub.Diagnostic);
                    }
                }

                if (DB != null)
                {

                    foreach (var item in TDiagnostics)
                    {
                        if (item != null)
                            DB.Update(Tables.CDiagnostic.TableName, item);
                    }

                    //DB.WhereNotInDelete(Tables.CDiagnostic.TableName, nameof(Tables.CRtValue.Tags_id), TRTvalues.Select(x => x.Tags_id).ToArray());

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
