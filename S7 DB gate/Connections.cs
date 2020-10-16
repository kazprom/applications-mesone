
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

                TagSettings tag = new TagSettings();
                TagSettings.SettingFromDataRow(e.Row, ref tag);
                
                lock (connections)
                {
                    switch (e.Action)
                    {
                        case DataRowAction.Add:
                        case DataRowAction.Change:
                            {
                                if (connections.ContainsKey(tag.clients_id))
                                    connections[tag.clients_id].PutTag(tag);
                                break;
                            }
                        case DataRowAction.Delete:
                            {
                                if (connections.ContainsKey(tag.clients_id))
                                    connections[tag.clients_id].RemoveTag(tag.id);
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
                ClientSettings settings = new ClientSettings();
                ClientSettings.SettingFromDataRow(e.Row, ref settings);

                lock (connections)
                {
                    switch (e.Action)
                    {
                        case DataRowAction.Add:
                        case DataRowAction.Change:
                            {
                                if (!connections.ContainsKey(settings.id))
                                    connections.Add(settings.id, new S7connection(settings, buffer, diagnostics));
                                connections[settings.id].Settings = settings;
                                break;
                            }
                        case DataRowAction.Delete:
                            {
                                connections[settings.id].Dispose();
                                connections.Remove(settings.id);
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