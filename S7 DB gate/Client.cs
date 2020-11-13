using LibDBgate;
using LibDBgate.Structs;
using S7_DB_gate.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Ubiety.Dns.Core.Records;

namespace S7_DB_gate
{
    public class Client : LibDBgate.Client
    {

        #region CONSTANTS

#if DEBUG
        private const int period = 5000;
#else
        private const int period = 30000;
#endif

        #endregion

        #region VARIABLES

        public S7.Net.Plc Plc;

        private Timer timer;

        #endregion

        #region PROPERTIES

        public S7.Net.CpuType CPU_type { get; set; }
        public string IP { get; set; }
        public uint Port { get; set; }
        public ushort Rack { get; set; }
        public ushort Slot { get; set; }


        #endregion

        #region CONSTRUCTOR

        public Client(Service parent, ulong id) : base(parent, id)
        {
            try
            {
                timer = new Timer(ConnectionHandler, null, 0, period);

            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Title}. Constructor");
            }
        }

        #endregion

        #region DESTRUCTOR

        protected override void Dispose(bool disposing)
        {

            if (disposing)
            {

                WaitHandle h = new AutoResetEvent(false);
                timer.Dispose(h);
                h.WaitOne();

                if (Plc != null && Plc.IsConnected)
                {
                    Plc.Close();
                    logger.Info($"{Title}. Closed connection");
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region PUBLICS


        public void LoadSettings(string name, string cpu_type, string ip, uint port, ushort rack, ushort slot)
        {
            try
            {
                Name = name;
                CPU_type = (S7.Net.CpuType)Enum.Parse(typeof(S7.Net.CpuType), cpu_type);
                IP = ip;
                Port = port;
                Rack = rack;
                Slot = slot;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Title}. Update settings");
            }
        }


        public override void TagsReader(object state)
        {

            try
            {

                IEnumerable<Structs.Tag> tags = Parent.Database.WhereRead<Structs.Tag>(Structs.Tag.TableName,
                                                                                       new { Enabled = true, Clients_id = ID });

                IEnumerable<ushort> fresh_rates = tags.GroupBy(x => x.Rate).Select(x => x.First()).Select(x => x.Rate);
                IEnumerable<ushort> existing_rates = this.Groups.Keys;

                IEnumerable<ushort> waste = existing_rates.Except(fresh_rates);
                IEnumerable<ushort> modify = fresh_rates.Intersect(existing_rates);
                IEnumerable<ushort> missing = fresh_rates.Except(existing_rates);

                foreach (ushort rate in waste)
                {
                    Groups[rate].Dispose();
                    Groups.Remove(rate);
                }

                foreach (ushort rate in modify)
                {
                    Groups[rate].LoadTags(tags.Where(x => x.Rate == rate));
                }

                foreach (ushort rate in missing)
                {
                    Group group = new Group(this, rate);
                    group.LoadTags(tags.Where(x => x.Rate == rate));
                    Groups.Add(rate, group);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Title}. Tags reader");
            }

        }

        #endregion

        #region PRIVATES

        private void ConnectionHandler(object state)
        {

            try
            {
                if (Plc == null)
                {
                    IPAddress ip_result;
                    if (IPAddress.TryParse(IP, out ip_result) && Port != 0)
                        Plc = new S7.Net.Plc(CPU_type, IP, (int)Port, (short)Rack, (short)Slot);
                    else
                    {
                        logger.Warn($"{Title}. Incorrect connection settings");
                        Diagnostic.State = "Wrong settings";
                        Diagnostic.Message = null;
                    }

                }

                if (Plc != null)
                {
                    lock (Plc)
                    {

                        if (Plc.CPU != CPU_type || Plc.IP != IP || Plc.Port != Port || Plc.Rack != Rack || Plc.Slot != Slot)
                        {
                            if (Plc.IsConnected)
                            {
                                Plc.Close();
                                logger.Info($"{Title}. Closed connection");
                                Diagnostic.State = "Disconnected";
                                Diagnostic.Message = null;
                            }

                            Plc = new S7.Net.Plc(CPU_type, IP, (int)Port, (short)Rack, (short)Slot);

                        }

                        if (!Plc.IsConnected)
                        {
                            try
                            {
                                Plc.Open();
                                logger.Info($"{Title}. Openned connection");
                                Diagnostic.State = "Connected";
                                Diagnostic.Message = null;
                            }
                            catch (Exception ex)
                            {
                                logger.Warn(ex, $"{Title}. Open connection");
                                Diagnostic.State = "Disconnected";
                                Diagnostic.Message = ex.Message;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Title}. Connection handler");
                Diagnostic.State = "Error connection";
                Diagnostic.Message = ex.Message;
            }
        }



        #endregion

    }
}
