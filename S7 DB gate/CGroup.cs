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
            try
            {
                lock (Tags)
                {
                    foreach (CTag tag in Tags.Values)
                    {
                        if (tag.RT_enabled || tag.History_enabled)
                        {

                            tag.Timestamp = DateTime.Now;
                            CClient parent = (CClient)Parent;

                            if (parent.Plc != null && parent.Plc.IsConnected)
                            {

                                object result = null;

                                lock (parent.Plc)
                                {
                                    try
                                    {
                                        result = parent.Plc.Read(tag.DataType,
                                                                 tag.DB,
                                                                 tag.StartByteAdr,
                                                                 tag.VarType,
                                                                 1,
                                                                 tag.BitAdr);

                                        if (result == null)
                                        {
                                            tag.Value = null;
                                            tag.Quality = LibDBgate.Tag.EQuality.Bad;

                                            logger.Warn($"{Title}. Tag is null");
                                        }
                                        else
                                        {
                                            tag.Value = LibDBgate.Tag.ObjToDataType(result, tag.TagType);
                                            tag.Quality = LibDBgate.Tag.EQuality.Good;

                                            if (tag.History_enabled)
                                            {
                                                parent.Parent.retro_buf.Enqueue(new LibDBgate.Structs.RetroValue()
                                                {
                                                    Tags_id = tag.ID,
                                                    Timestamp = (DateTime)tag.Timestamp,
                                                    Value = LibDBgate.Tag.ObjToBin(tag.Value),
                                                    Quality = (byte)tag.Quality
                                                });
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        tag.Value = null;
                                        tag.Quality = (byte)LibDBgate.Tag.EQuality.Bad;

                                        logger.Error(ex, $"{Title}. Read tag");
                                    }
                                }
                            }
                            else
                            {
                                tag.Value = null;
                                tag.Quality = (byte)LibDBgate.Tag.EQuality.Bad;
                            }

                        }
                        else
                        {
                            tag.Timestamp = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Title}. Handler");
            }

        }


        #endregion
        */


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

                            if (parent.Plc != null && parent.Plc.IsConnected)
                            {

                                object result = null;

                                lock (parent.Plc)
                                {
                                    try
                                    {
                                        result = parent.Plc.Read(tag.DataType,
                                                                 tag.DB,
                                                                 tag.StartByteAdr,
                                                                 tag.VarType,
                                                                 1,
                                                                 tag.BitAdr);

                                        if (result == null)
                                        {
                                            tag.Value = null;
                                            tag.Quality = LibDBgate.Tag.EQuality.Bad;

                                            logger.Warn($"{Title}. Tag is null");
                                        }
                                        else
                                        {
                                            tag.Value = LibDBgate.Tag.ObjToDataType(result, tag.TagType);
                                            tag.Quality = LibDBgate.Tag.EQuality.Good;

                                            if (tag.History_enabled)
                                            {
                                                parent.Parent.retro_buf.Enqueue(new LibDBgate.Structs.RetroValue()
                                                {
                                                    Tags_id = tag.ID,
                                                    Timestamp = (DateTime)tag.Timestamp,
                                                    Value = LibDBgate.Tag.ObjToBin(tag.Value),
                                                    Quality = (byte)tag.Quality
                                                });
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        tag.Value = null;
                                        tag.Quality = (byte)LibDBgate.Tag.EQuality.Bad;

                                        logger.Error(ex, $"{Title}. Read tag");
                                    }
                                }
                            }
                            else
                            {
                                tag.Value = null;
                                tag.Quality = (byte)LibDBgate.Tag.EQuality.Bad;
                            }

                        }
                        else
                        {
                            tag.Timestamp = null;
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
