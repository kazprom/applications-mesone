using LibMESone;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Timers;

namespace LibPlcDBgate
{
    public class CClient : LibDBgate.CSUB
    {

        public override ulong Id
        {
            get => base.Id;
            set
            {
                if (!Equals(base.Id, value))
                {
                    base.Id = value;
                    Diagnostic = new Tables.CDiagnostic() { Clients_id = Id };
                }
            }
        }

        protected IPAddress ip;
        public dynamic Ip
        {
            get { return ip != null ? ip.ToString() : null; }
            set
            {
                try
                {
                    ip = IPAddress.Parse(Convert.ToString(value));
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
            }
        }

        protected ushort? port;
        public dynamic Port
        {
            get { return port; }
            set
            {
                try
                {
                    port = ushort.Parse(Convert.ToString(value));
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
            }
        }

        public virtual dynamic Tags { get; set; }


        public CClient() : base()
        {
            CycleRate = 10000;
        }




    }
}
