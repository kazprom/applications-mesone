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

        public const string col_name_ip = "ip";
        public const string col_name_port = "port";

        #endregion

        #region VARIABLES



        #endregion


        #region PROPERTIES

        private DBTable source = new DBTable("clients");
        public DBTable Source { get { return source; } }

        #endregion


        public Clients()
        {

            source.AddColumn(col_name_ip, typeof(string));
            source.AddColumn(col_name_port, typeof(int));

        }

    }
}
