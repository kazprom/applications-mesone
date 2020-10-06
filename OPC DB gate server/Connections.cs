using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text;

namespace OPC_DB_gate_server
{
    class Connections
    {


        #region VARIABLES

        private Clients clients;
        private Tags tags;
        private Lib.Buffer<OPC_DB_gate_Lib.TagData> buffer;

        private Dictionary<int, TCPconnection> connections = new Dictionary<int, TCPconnection>();

        #endregion

        public Connections(Clients clients, Tags tags, Lib.Buffer<OPC_DB_gate_Lib.TagData> buffer)
        {
            this.clients = clients;
            this.clients.Source.Table.RowChanged += ClientsHandler;
            this.clients.Source.Table.RowDeleting += ClientsHandler;

            this.tags = tags;
            this.tags.Source.Table.RowChanged += TagsHandler;
            this.tags.Source.Table.RowDeleting += TagsHandler;

            this.buffer = buffer;

        }


        private void ClientsHandler(object sender, DataRowChangeEventArgs e)
        {
            try
            {
                int id = (int)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(Lib.DBTable.col_name_id)];
                IPAddress ip = IPAddress.Parse((string)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(Clients.col_name_ip)]);
                int port = (int)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(Clients.col_name_port)];

                lock (connections)
                {
                    switch (e.Action)
                    {
                        case DataRowAction.Add:
                        case DataRowAction.Change:
                            {
                                if (!connections.ContainsKey(id))
                                    connections.Add(id, new TCPconnection(buffer));
                                connections[id].Settings(ip, port);
                                break;
                            }
                        case DataRowAction.Delete:
                            {
                                connections[id].Dispose();
                                connections.Remove(id);
                                break;
                            }
                    }
                }

            }
            catch (Exception ex)
            {

                throw new Exception("Error handling clients", ex);
            }
        }

        private void TagsHandler(object sender, DataRowChangeEventArgs e)
        {
            try
            {

                int id = (int)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(Lib.DBTable.col_name_id)];
                int clients_id = (int)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(Tags.col_name_clients_id)];

                OPC_DB_gate_Lib.TagSettings tag = new OPC_DB_gate_Lib.TagSettings()
                {
                    path = (string)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(Tags.col_name_path)],
                    rate = (int)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(Tags.col_name_rate)],
                    data_type = (OPC_DB_gate_Lib.TagSettings.EDataType)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(Tags.col_name_data_type)]
                };

                lock (connections)
                {
                    switch (e.Action)
                    {
                        case DataRowAction.Add:
                        case DataRowAction.Change:
                            {
                                if (connections.ContainsKey(clients_id))
                                    connections[clients_id].PutTag(id, tag);
                                break;
                            }
                        case DataRowAction.Delete:
                            {
                                if (connections.ContainsKey(clients_id))
                                    connections[clients_id].RemoveTag(id);
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error handling tags", ex);
            }
        }

    }
}
