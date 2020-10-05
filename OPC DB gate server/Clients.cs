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


        private Dictionary<int, TCPconnection> tcp_connections = new Dictionary<int, TCPconnection>();
        public Dictionary<int, TCPconnection> TCPconnections { get { return tcp_connections; } }

        private Dictionary<int, Dictionary<int, OPC_DB_gate_Lib.TagSettings>> tag_groups;
        public Dictionary<int, Dictionary<int, OPC_DB_gate_Lib.TagSettings>> TagGroups { get; set; }

        #endregion



        public Clients(Dictionary<int, Dictionary<int, OPC_DB_gate_Lib.TagSettings>> tag_groups)
        {

            this.tag_groups = tag_groups;

            source.AddColumn(col_name_ip, typeof(string));
            source.AddColumn(col_name_port, typeof(int));

            source.Table.RowChanged += TCPconnectionHandler;
            source.Table.RowDeleting += TCPconnectionHandler;

        }


        #region PRIVATES

        private void TCPconnectionHandler(object sender, DataRowChangeEventArgs e)
        {
            try
            {
                int id = (int)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(DBTable.EColumns.id.ToString())];
                IPAddress ip = IPAddress.Parse((string)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(col_name_ip)]);
                int port = (int)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(col_name_port)];

                lock (tcp_connections)
                {
                    switch (e.Action)
                    {
                        case DataRowAction.Add:
                            {
                                tcp_connections.Add(id, new TCPconnection(id, ip, port, tag_groups));
                                break;
                            }
                        case DataRowAction.Change:
                            {
                                tcp_connections[id].Settings(ip, port);
                                break;
                            }
                        case DataRowAction.Delete:
                            {
                                tcp_connections[id].Dispose();
                                tcp_connections.Remove(id);
                                break;
                            }
                    }
                }

            }
            catch (Exception ex)
            {

                throw new Exception("Error handling of TCP connection", ex);
            }
        }

        #endregion


    }
}
