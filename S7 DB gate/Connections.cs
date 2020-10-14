
using System;
using System.Collections.Generic;
using System.Data;

namespace S7_DB_gate
{
    public class Connections
    {
        private Clients clients;
        private Tags tags;
        private Lib.Buffer<LibDBgate.TagData> buffer;
        private Diagnostics diagnostics;

        private Dictionary<int, S7connection> connections = new Dictionary<int, S7connection>();

        public Connections(Clients clients, Tags tags, Lib.Buffer<LibDBgate.TagData> buffer, Diagnostics diagnostics)
        {
            try
            {
                this.clients = clients;
                this.clients.Source.RowChanged += ClientsHandler;
                this.clients.Source.RowDeleting += ClientsHandler;

                this.tags = tags;
                this.tags.Source.RowChanged += TagsHandler;
                this.tags.Source.RowDeleting += TagsHandler;

                this.buffer = buffer;
                this.diagnostics = diagnostics;

            }
            catch (Exception ex)
            {
                throw new Exception("Error constructor", ex);
            }
        }

        private void TagsHandler(object sender, DataRowChangeEventArgs e)
        {

            try
            {

                int id = (int)e.Row[Tags.col_name_id];
                int clients_id = (int)e.Row[Tags.col_name_clients_id];

                TagSettings tag = new TagSettings()
                {
                    plc_data_type = (S7.Net.DataType)Enum.Parse(typeof(S7.Net.DataType), (string)e.Row[Tags.col_name_plc_data_type]),
                    db_no = (int)e.Row[Tags.col_name_db_no],
                    db_offset = (int)e.Row[Tags.col_name_db_offset],
                    rate = (int)e.Row[Tags.col_name_rate],
                    req_type = (S7.Net.VarType)Enum.Parse(typeof(S7.Net.VarType), (string)e.Row[Tags.col_name_req_type]),
                    data_type = (LibDBgate.TagData.EDataType)e.Row[Tags.col_name_data_type],
                    rt_value_enabled = (bool)e.Row[Tags.col_name_rt_values_enabled],
                    history_enabled = (bool)e.Row[Tags.col_name_history_enabled]
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
                throw new Exception("Error tags handler", ex);
            }

        }

        private void ClientsHandler(object sender, DataRowChangeEventArgs e)
        {

            try
            {
                int id = (int)e.Row[Clients.col_name_id];
                S7.Net.CpuType cpu_type = (S7.Net.CpuType)Enum.Parse(typeof(S7.Net.CpuType), (string)e.Row[Clients.col_name_cpu_type]);
                string ip = (string)e.Row[Clients.col_name_ip];
                short port = (short)e.Row[Clients.col_name_port];
                short rack = (short)e.Row[Clients.col_name_rack];
                short slot = (short)e.Row[Clients.col_name_slot];

                lock (connections)
                {
                    switch (e.Action)
                    {
                        case DataRowAction.Add:
                        case DataRowAction.Change:
                            {
                                if (!connections.ContainsKey(id))
                                    connections.Add(id, new S7connection(id, buffer, diagnostics));
                                connections[id].Settings(cpu_type, ip, port, rack, slot);
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

                throw new Exception("Error clients handler", ex);
            }

        }
    }
}