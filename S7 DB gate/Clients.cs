using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace S7_DB_gate
{
    public class Clients
    {


        #region CONSTANTS

        public const string col_name_id = "id";
        public const string col_name_cpu_type = "cpu_type";
        public const string col_name_ip = "ip";
        public const string col_name_port = "port";
        public const string col_name_rack = "rack";
        public const string col_name_slot = "slot";

        #endregion

        #region PROPERTIES

        private DataTable source = new DataTable("clients");
        public DataTable Source { get { return source; } }

        #endregion


        public Clients()
        {
            try
            {
                source.Columns.Add(col_name_id, typeof(int)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp() { primary_key = true });
                source.Columns.Add(col_name_cpu_type, typeof(string));
                source.Columns.Add(col_name_ip, typeof(string));
                source.Columns.Add(col_name_port, typeof(short));
                source.Columns.Add(col_name_rack, typeof(short));
                source.Columns.Add(col_name_slot, typeof(short));
            }
            catch (Exception ex)
            {
                throw new Exception("Error constructor", ex);
            }
        }

    }
}
