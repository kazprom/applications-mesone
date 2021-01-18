using LibMESone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

namespace S7_DB_gate
{
    public class CGroup : LibPlcDBgate.CGroup
    {

        public override dynamic Tags
        {
            set
            {
                try
                {

                    var data = from tags in (IEnumerable<dynamic>)value
                               select new
                               {

                                   Parent = this,
                                   tags.Id,
                                   tags.Name,

                                   tags.Data_type,
                                   tags.History_enabled,
                                   RT_enabled = tags.RT_values_enabled,

                                   tags.S7_Data_Type,
                                   tags.DB,
                                   tags.StartByteAdr,
                                   tags.BitAdr,
                                   tags.S7_Var_Type
                               };



                    Dictionary<ulong, Dictionary<string, object>> children_props = data.ToDictionary(o => (ulong)o.Id,
                                                                                                     o => o.
                                                                                                          GetType().
                                                                                                          GetProperties().ToDictionary(z => z.Name,
                                                                                                                                       z => z.GetValue(o)));

                    CUD<CTag>(children_props);

                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

            }
        }


        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {

            try
            {

                lock (Children)
                {

                    foreach (CTag tag in Children.Values)
                    {
                        if (tag != null && (tag.RT_enabled || tag.History_enabled))
                        {

                            //tag.Timestamp = DateTime.Now;
                            CClient parent = (CClient)Parent;

                            if (parent.plc != null && parent.plc.IsConnected)
                            {

                                object result = null;

                                lock (parent.plc)
                                {
                                    try
                                    {
                                        result = parent.plc.Read(tag.S7_Data_Type,
                                                                 tag.DB,
                                                                 tag.StartByteAdr,
                                                                 tag.S7_Var_Type,
                                                                 1,
                                                                 tag.BitAdr);


                                        if (result == null)
                                        {
                                            tag.Quality = LibPlcDBgate.CTag.EQuality.Bad;
                                        }
                                        else
                                        {
                                            tag.Quality = LibPlcDBgate.CTag.EQuality.Good;
                                        }

                                        tag.Value = result;

                                    }
                                    catch (Exception)
                                    {
                                        tag.Quality = LibPlcDBgate.CTag.EQuality.Uncertain;
                                        tag.Value = null;
                                    }
                                }
                            }
                            else
                            {
                                tag.Quality = LibPlcDBgate.CTag.EQuality.Bad_Comm_Failure;
                                tag.Value = null;
                            }

                        }
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
