using LibDBgate;
using S7_DB_gate.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace S7_DB_gate
{
    public class S7Client : IDisposable
    {
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region VARIABLES

        private string name;

        private S7.Net.Plc plc;
        private S7.Net.CpuType cpu_type;
        private string ip;
        private int port;
        private short rack;
        private short slot;

        private Timer connection_handler;


        private Dictionary<short, Reader> readers = new Dictionary<short, Reader>();


        #endregion

        #region CONSTRUCTOR

        public S7Client(string name)
        {


            try
            {
                logger.Info(name);
                this.name = name;
                
                connection_handler = new Timer(ConnectionHandler, null, 0, 10000);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        #endregion

        #region DESTRUCTOR

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                // TODO: установить значение NULL для больших полей
                disposedValue = true;
                logger.Info(name);

            }
        }

        // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        // ~S7connection()
        // {
        //     // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
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
            catch (Exception)
            {

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
                        Reader reader = new Reader(plc, rate);

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
                logger.Error(ex);
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

                if (plc != null && !plc.IsConnected)
                {
                    plc.Open();
                }
            }
            catch (Exception)
            {

            }

        }

        private class Reader : IDisposable
        {

            private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

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
            }

            #endregion


            #region VARIABLES

            private S7.Net.Plc plc;
            private short rate;
            private Dictionary<long, STag> tags = new Dictionary<long, STag>();
            private Timer timer;

            #endregion


            #region CONSTRUCTOR

            public Reader(S7.Net.Plc plc, short rate)
            {

                try
                {
                    logger.Info($"Created group [{rate}ms]");

                    this.plc = plc;
                    this.rate = rate;
                    timer = new Timer(Handler, null, 0, this.rate);

                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }

            #endregion


            #region DESTRUCTOR

            private bool disposedValue;
            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: освободить управляемое состояние (управляемые объекты)
                    }

                    // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                    // TODO: установить значение NULL для больших полей
                    disposedValue = true;
                    logger.Info($"Removed group [{rate}ms]");
                }
            }

            // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
            // ~Reader()
            // {
            //     // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            //     Dispose(disposing: false);
            // }

            public void Dispose()
            {
                // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
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
                                logger.Info($"Group [{rate}] Tag [{this.tags[tag_id].name}] removed");
                                this.tags.Remove(tag_id);
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);
                            }
                        }

                        foreach (long tag_id in modify)
                        {
                            try
                            {
                                STag inst_tag = this.tags[tag_id];
                                Tag set_tag = tags.First(x => x.Id == tag_id);

                                inst_tag.name = set_tag.Name;
                                inst_tag.dataType = (S7.Net.DataType)Enum.Parse(typeof(S7.Net.DataType), set_tag.PLC_data_type);
                                inst_tag.db = set_tag.Datablock_no;
                                inst_tag.startByteAdr = set_tag.Datablock_offset;
                                inst_tag.varType = (S7.Net.VarType)Enum.Parse(typeof(S7.Net.VarType), set_tag.Req_type);
                                inst_tag.bitAdr = set_tag.Bit_offset;

                                this.tags[tag_id] = inst_tag;
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);
                            }
                        }

                        foreach (long tag_id in missing)
                        {

                            try
                            {

                                STag inst_tag = new STag();
                                Tag set_tag = tags.First(x => x.Id == tag_id);

                                inst_tag.id = set_tag.Id;
                                inst_tag.name = set_tag.Name;
                                inst_tag.dataType = (S7.Net.DataType)Enum.Parse(typeof(S7.Net.DataType), set_tag.PLC_data_type);
                                inst_tag.db = set_tag.Datablock_no;
                                inst_tag.startByteAdr = set_tag.Datablock_offset;
                                inst_tag.varType = (S7.Net.VarType)Enum.Parse(typeof(S7.Net.VarType), set_tag.Req_type);
                                inst_tag.bitAdr = set_tag.Bit_offset;

                                this.tags.Add(tag_id, inst_tag);
                                logger.Info($"Group [{rate}] Tag [{this.tags[tag_id].name}] added");

                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);
                            }
                        }
                    }
                }
                catch (Exception)
                {


                }
            }

            #endregion


            #region PRIVATES

            private void Handler(object state)
            {
                try
                {
                    if (plc != null && plc.IsConnected)
                    {
                        lock (tags)
                        {
                            foreach (STag tag in tags.Values)
                            {
                                try
                                {
                                    object result = plc.Read(tag.dataType,
                                                             tag.db,
                                                             tag.startByteAdr,
                                                             tag.varType,
                                                             1,
                                                             tag.bitAdr);

                                    Console.WriteLine(result);

                                }
                                catch (Exception)
                                {

                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
            }


            #endregion

        }

        #endregion
    }
}
