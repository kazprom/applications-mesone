using LibDBgate;
using S7_DB_gate.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Ubiety.Dns.Core.Records;

namespace S7_DB_gate
{
    public class S7Client : IDisposable
    {
        #region VARIABLES

        public readonly string title;

        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private LibDBgate.Service parent;
        private string name;

        private S7.Net.Plc plc;
        private S7.Net.CpuType cpu_type;
        private string ip;
        private int port;
        private short rack;
        private short slot;

        private Timer timer;


        private Dictionary<short, Reader> readers = new Dictionary<short, Reader>();


        #endregion

        #region CONSTRUCTOR

        public S7Client(LibDBgate.Service parent, string name, string cpu_type, string ip, int port, short rack, short slot)
        {
            try
            {

                this.parent = parent;
                this.name = name;

                title = $"{this.parent.title} client [{this.name}]";

                UpdateSettings(cpu_type, ip, port, rack, slot);

                timer = new Timer(ConnectionHandler, null, 0, 10000);

                logger.Info($"{title} added");
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{title} add");
            }
        }

        #endregion

        #region DESTRUCTOR

        ~S7Client()
        {
            logger.Info($"{title} removed");
        }


        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion


        #region PUBLICS


        public void UpdateSettings(string cpu_type, string ip, int port, short rack, short slot)
        {
            try
            {
                this.cpu_type = (S7.Net.CpuType)Enum.Parse(typeof(S7.Net.CpuType), cpu_type);
                this.ip = ip;
                this.port = port;
                this.rack = rack;
                this.slot = slot;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{title} update settings");
            }
        }

        public void LoadTags(IEnumerable<Structs.Tag> tags)
        {
            try
            {
                IEnumerable<short> fresh_rates = tags.GroupBy(x => x.Rate).Select(x => x.First()).Select(x => x.Rate);
                IEnumerable<short> existing_rates = this.readers.Keys;

                IEnumerable<short> waste = existing_rates.Except(fresh_rates);
                IEnumerable<short> modify = fresh_rates.Intersect(existing_rates);
                IEnumerable<short> missing = fresh_rates.Except(existing_rates);

                foreach (short rate in waste)
                {
                    try
                    {
                        readers[rate].Dispose();
                        readers.Remove(rate);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }

                }

                foreach (short rate in modify)
                {
                    try
                    {
                        readers[rate].LoadTags(tags.Where(x => x.Rate == rate));
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }

                foreach (short rate in missing)
                {
                    try
                    {
                        Reader reader = new Reader(this, rate);

                        reader.LoadTags(tags.Where(x => x.Rate == rate));
                        readers.Add(rate, reader);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{title} load tags");
            }
        }

        #endregion


        #region PRIVATES

        private void ConnectionHandler(object state)
        {

            try
            {
                if (plc == null)
                {
                    plc = new S7.Net.Plc(cpu_type, ip, port, rack, slot);
                }

                if (plc != null)
                {
                    lock (plc)
                    {

                        if (plc.CPU != cpu_type || plc.IP != ip || plc.Port != port || plc.Rack != rack || plc.Slot != slot)
                        {
                            if (plc.IsConnected)
                            {
                                plc.Close();
                                logger.Info($"{title} closed connection");
                            }

                            plc = new S7.Net.Plc(cpu_type, ip, port, rack, slot);

                        }

                        if (!plc.IsConnected)
                        {
                            try
                            {
                                plc.Open();
                                logger.Info($"{title} openned connection");
                            }
                            catch (Exception ex)
                            {
                                logger.Warn(ex, $"{title} open connection");
                            }
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{title} connection handler");
            }

        }

        private class Reader : IDisposable
        {

            #region STRUCTS

            private struct STag
            {
                public long id;
                public string name;
                public S7.Net.DataType dataType;
                public int db;
                public int startByteAdr;
                public S7.Net.VarType varType;
                public byte bitAdr;
                public TagData.EDataType tagType;
                public bool rt_enabled;
                public bool history_enabled;
            }

            #endregion

            #region VARIABLES

            public readonly string title;

            private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

            private S7Client parent;
            private short rate;
            private Dictionary<long, STag> tags = new Dictionary<long, STag>();

            private Timer timer;

            #endregion

            #region CONSTRUCTOR

            public Reader(S7Client parent, short rate)
            {

                try
                {

                    this.parent = parent;
                    this.rate = rate;

                    title = $"{parent.title} group [{rate}]";

                    timer = new Timer(Handler, null, 0, rate);

                    logger.Info($"{title} added");

                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }

            #endregion

            #region DESTRUCTOR

            ~Reader()
            {
                logger.Info($"{title} removed");
            }

            private bool disposedValue;
            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        IEnumerable<long> tags_ids = this.tags.Keys;

                        foreach (long tag_id in tags_ids)
                        {
                            try
                            {
                                logger.Info($"{title} tag [{this.tags[tag_id].name}] disposed");
                                this.tags.Remove(tag_id);
                            }
                            catch (Exception ex)
                            {
                                logger.Warn(ex);
                            }
                        }

                    }

                    disposedValue = true;
                }
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }

            #endregion

            #region PUBLICS

            public void LoadTags(IEnumerable<Structs.Tag> tags)
            {
                try
                {
                    lock (this.tags)
                    {

                        IEnumerable<long> fresh_ids = tags.Select(x => x.Id);
                        IEnumerable<long> existing_ids = this.tags.Keys;

                        IEnumerable<long> waste = existing_ids.Except(fresh_ids);
                        IEnumerable<long> modify = fresh_ids.Intersect(existing_ids);
                        IEnumerable<long> missing = fresh_ids.Except(existing_ids);

                        foreach (long tag_id in waste)
                        {
                            try
                            {
                                logger.Info($"{title} tag [{this.tags[tag_id].name}] removed");
                                this.tags.Remove(tag_id);
                            }
                            catch (Exception ex)
                            {
                                logger.Warn(ex, $"{title} tag [{tag_id}] remove");
                            }
                        }

                        foreach (long tag_id in modify)
                        {
                            try
                            {
                                STag tag = this.tags[tag_id];
                                if (Converter(tags.First(x => x.Id == tag_id), ref tag))
                                {
                                    this.tags[tag_id] = tag;
                                    logger.Info($"{title} tag [{this.tags[tag_id].name}] changed");
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Warn(ex, $"{title} tag [{tag_id}] change");
                            }
                        }

                        foreach (long tag_id in missing)
                        {

                            try
                            {
                                STag tag = new STag();
                                Converter(tags.First(x => x.Id == tag_id), ref tag);
                                this.tags.Add(tag_id, tag);
                                logger.Info($"{title} tag [{this.tags[tag_id].name}] added");
                            }
                            catch (Exception ex)
                            {
                                logger.Warn(ex, $"{title} tag [{tag_id}] add");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"{title} load tags");
                }
            }

            #endregion

            #region PRIVATES

            private void Handler(object state)
            {
                try
                {
                    lock (tags)
                    {
                        foreach (STag tag in tags.Values)
                        {

                            DateTime ts = DateTime.Now;
                            if (parent.plc != null && parent.plc.IsConnected)
                            {
                                try
                                {


                                    if (tag.rt_enabled || tag.history_enabled)
                                    {

                                        try
                                        {

                                            object result = null;

                                            lock (parent.plc)
                                            {
                                                result = parent.plc.Read(tag.dataType,
                                                                         tag.db,
                                                                         tag.startByteAdr,
                                                                         tag.varType,
                                                                         1,
                                                                         tag.bitAdr);
                                            }


                                            if (result != null)
                                            {
                                                object value = TagData.ObjToDataType(result, tag.tagType);
                                                byte[] value_raw = TagData.ObjToBin(value);
                                                byte quality = (byte)TagData.EQuality.Good;


                                                if (tag.rt_enabled)
                                                {
                                                    parent.parent.rt_buf.Enqueue(new LibDBgate.Structs.RT_values()
                                                    {
                                                        Tags_id = tag.id,
                                                        Timestamp = ts,
                                                        Value_raw = value_raw,
                                                        Value_str = value.ToString(),
                                                        Quality = quality
                                                    });
                                                }

                                                if (tag.history_enabled)
                                                {
                                                    parent.parent.his_buf.Enqueue(new LibDBgate.Structs.History()
                                                    {
                                                        Tags_id = tag.id,
                                                        Timestamp = ts,
                                                        Value = value_raw,
                                                        Quality = quality
                                                    });
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                            if (tag.rt_enabled)
                                                parent.parent.rt_buf.Enqueue(new LibDBgate.Structs.RT_values()
                                                {
                                                    Tags_id = tag.id,
                                                    Timestamp = ts,
                                                    Quality = (byte)TagData.EQuality.Bad
                                                });

                                            logger.Warn(ex, $"read tag[{tag.id}]");

                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Warn(ex, $"{title} read");
                                }
                            }
                            else
                            {

                                if (tag.rt_enabled)
                                    parent.parent.rt_buf.Enqueue(new LibDBgate.Structs.RT_values()
                                    {
                                        Tags_id = tag.id,
                                        Timestamp = ts,
                                        Quality = (byte)TagData.EQuality.Bad
                                    });

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"{title} handler");
                }


            }

            private bool Converter(Tag input, ref STag output)
            {
                bool result = false;
                string title = $"Tag";

                if (input != null)
                {
                    try
                    {

                        title += $" ID [{input.Id}]";
                        if (output.id != input.Id)
                        {
                            output.id = input.Id;
                            result = true;
                        }

                        title += $" Name [{input.Name}]";
                        if (output.name != input.Name)
                        {
                            output.name = input.Name;
                            result = true;
                        }

                        title += $" PLC data type [{input.PLC_data_type}]";
                        S7.Net.DataType res_dt = (S7.Net.DataType)Enum.Parse(typeof(S7.Net.DataType), input.PLC_data_type, true);
                        if (output.dataType != res_dt)
                        {
                            output.dataType = res_dt;
                            result = true;
                        }


                        title += $" Data block no. [{input.Data_block_no}]";
                        if (output.db != input.Data_block_no)
                        {
                            output.db = input.Data_block_no;
                            result = true;
                        }

                        title += $" Data block offset [{input.Data_block_offset}]";
                        if (output.startByteAdr != input.Data_block_offset)
                        {
                            output.startByteAdr = input.Data_block_offset;
                            result = true;
                        }

                        title += $" Request type [{input.Request_type}]";
                        S7.Net.VarType res_vt = (S7.Net.VarType)Enum.Parse(typeof(S7.Net.VarType), input.Request_type, true);
                        if (output.varType != res_vt)
                        {
                            output.varType = res_vt;
                            result = true;
                        }

                        title += $" Bit offset [{input.Bit_offset}]";
                        if (output.bitAdr != input.Bit_offset)
                        {
                            output.bitAdr = input.Bit_offset;
                            result = true;
                        }

                        title += $" Data type [{input.Data_type}]";
                        TagData.EDataType res_sdt = (TagData.EDataType)Enum.Parse(typeof(TagData.EDataType), input.Data_type, true);
                        if (output.tagType != res_sdt)
                        {
                            output.tagType = res_sdt;
                            result = true;
                        }

                        if (output.rt_enabled != input.RT_values_enabled)
                        {
                            output.rt_enabled = input.RT_values_enabled;
                            result = true;
                        }

                        if (output.history_enabled != input.History_enabled)
                        {
                            output.history_enabled = input.History_enabled;
                            result = true;
                        }


                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"{title}. Error converter", ex);
                    }

                }
                return result;
            }

            #endregion
        }

        #endregion
    }
}
