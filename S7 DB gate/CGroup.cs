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
                                   tags.RT_values_enabled,

                                   tags.PLC_data_type,
                                   tags.Data_block_no,
                                   tags.Data_block_offset,
                                   tags.Bit_offset,
                                   tags.Request_type
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
                        if (tag.RT_enabled || tag.History_enabled)
                        {

                            tag.Timestamp = DateTime.Now;
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


        /*
        #region PUBLICS

        public override void LoadTags(dynamic tags)
        {
            try
            {
                IEnumerable<Structs.Tag> _tags = tags;

                lock (Tags)
                {

                    IEnumerable<ulong> fresh_ids = _tags.Select(x => x.Id);
                    IEnumerable<ulong> existing_ids = Tags.Keys;

                    IEnumerable<ulong> waste = existing_ids.Except(fresh_ids);
                    IEnumerable<ulong> modify = fresh_ids.Intersect(existing_ids);
                    IEnumerable<ulong> missing = fresh_ids.Except(existing_ids);

                    foreach (ulong tag_id in waste)
                    {
                        Tags[tag_id].Dispose();
                        Tags.Remove(tag_id);
                    }

                    foreach (ulong tag_id in modify)
                    {
                        CTag tag = (CTag)Tags[tag_id];
                        tag.LoadSettings(_tags.First(x => x.Id == tag_id));
                    }

                    foreach (ulong tag_id in missing)
                    {

                        CTag tag = new CTag(this, tag_id);
                        tag.LoadSettings(_tags.First(x => x.Id == tag_id));
                        Tags.Add(tag_id, tag);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Title}. Load tags");
            }
        }

        #endregion

        #region PRIVATES

        public override void Handler(object state)
        {
            

        }


        #endregion
        */

        /*
        public override void LoadSetting(ISetting setting)
        {
            base.LoadSetting(setting);

            try
            {
                CUD<CTag>(Settings.Tags);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }

        */

        /*
        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {

   


            base.Timer_Handler(sender, e);
        }
        */

    }
}
