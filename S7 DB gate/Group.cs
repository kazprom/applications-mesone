using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace S7_DB_gate
{
    public class Group : LibDBgate.Group
    {

        #region CONSTRUCTOR

        public Group(Client parent, ushort rate) : base(parent, rate)
        {

        }

        #endregion


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
                        Tag tag = (Tag)Tags[tag_id];
                        tag.LoadSettings(_tags.First(x => x.Id == tag_id));
                    }

                    foreach (ulong tag_id in missing)
                    {

                        Tag tag = new Tag(this, tag_id);
                        tag.LoadSettings(_tags.First(x => x.Id == tag_id));
                        this.Tags.Add(tag_id, tag);
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
                    foreach (Tag tag in Tags.Values)
                    {

                        DateTime ts = DateTime.Now;
                        Client parent = (Client)Parent;

                        if (parent.Plc != null && parent.Plc.IsConnected)
                        {
                            try
                            {


                                if (tag.RT_enabled || tag.History_enabled)
                                {

                                    try
                                    {

                                        object result = null;

                                        lock (parent.Plc)
                                        {
                                            result = parent.Plc.Read(tag.DataType,
                                                                     tag.DB,
                                                                     tag.StartByteAdr,
                                                                     tag.VarType,
                                                                     1,
                                                                     tag.BitAdr);
                                        }


                                        if (result != null)
                                        {
                                            tag.Value = LibDBgate.Tag.ObjToDataType(result, tag.TagType);
                                            tag.Quality = LibDBgate.Tag.EQuality.Good;
                                            tag.Timestamp = ts;

                                            if (tag.History_enabled)
                                            {
                                                parent.Parent.retro_buf.Enqueue(new LibDBgate.Structs.RetroValue()
                                                {
                                                    Tags_id = tag.ID,
                                                    Timestamp = ts,
                                                    Value = LibDBgate.Tag.ObjToBin(tag.Value),
                                                    Quality = (byte)tag.Quality
                                                });
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                        tag.Timestamp = ts;
                                        tag.Quality = (byte)LibDBgate.Tag.EQuality.Bad;

                                        logger.Warn(ex, $"{Title}. Handler");

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Warn(ex, $"{Title}. Handler");
                            }
                        }
                        else
                        {

                            tag.Timestamp = ts;
                            tag.Quality = (byte)LibDBgate.Tag.EQuality.Bad;

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



    }
}
