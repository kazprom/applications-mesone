using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace OPC_DB_gate_server
{
    public class Clients
    {


        #region CONSTANTS

        public const string col_name_id = "id";
        public const string col_name_ip = "ip";
        public const string col_name_port = "port";

        #endregion

        #region PROPERTIES

        private DataTable source = new DataTable("clients");
        public DataTable Source { get { return source; } }

        #endregion


        public Clients()
        {
            try
            {
                source.Columns.Add(col_name_id, typeof(int)).ExtendedProperties.Add(typeof(Lib.CDatabase.SExtProp), new Lib.CDatabase.SExtProp() { primary_key = true });
                source.Columns.Add(col_name_ip, typeof(string));
                source.Columns.Add(col_name_port, typeof(int));
            }
            catch (Exception ex)
            {
                throw new Exception("Error constructor", ex);
            }
        }

    }
}
