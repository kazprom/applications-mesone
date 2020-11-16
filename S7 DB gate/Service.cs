using Lib;
using LibMESone.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace S7_DB_gate
{
    public class Service : LibDBgate.Service
    {

        #region CONSTRUCTOR
        public Service(LibMESone.Core parent, ulong id) : base(parent, id)
        {

        }

        #endregion

        #region PUBLICS

        public override void DatabaseReadHandler(object state)
        {

            try
            {
                if (Database != null)
                {

                    IEnumerable<Structs.Client> clients = null;
                    if (Database.CompareTableSchema<Structs.Client>(Structs.Client.TableName))
                        clients = Database.WhereRead<Structs.Client>(Structs.Client.TableName, new { Enabled = true });

                    if (clients != null)
                    {

                        IEnumerable<ulong> fresh_ids = clients.Select(x => (ulong)x.Id);
                        IEnumerable<ulong> existing_ids = this.Clients.Keys;

                        IEnumerable<ulong> waste = existing_ids.Except(fresh_ids);
                        IEnumerable<ulong> modify = fresh_ids.Intersect(existing_ids);
                        IEnumerable<ulong> missing = fresh_ids.Except(existing_ids);

                        foreach (ulong point_id in waste)
                        {
                            Client srv = (Client)this.Clients[point_id];
                            srv.Dispose();
                            this.Clients.Remove(point_id);
                        }

                        foreach (ulong point_id in modify)
                        {
                            Structs.Client set_point = clients.First(x => x.Id == point_id);
                            Client srv = (Client)Clients[point_id];

                            srv.LoadSettings(set_point.Name,
                                             set_point.Cpu_type,
                                             set_point.Ip,
                                             set_point.Port,
                                             set_point.Rack,
                                             set_point.Slot);
                        }

                        foreach (ulong point_id in missing)
                        {
                            Structs.Client set_point = clients.First(x => x.Id == point_id);
                            Client inst_point = new Client(this, point_id);

                            inst_point.LoadSettings(set_point.Name,
                                                    set_point.Cpu_type,
                                                    set_point.Ip,
                                                    set_point.Port,
                                                    set_point.Rack,
                                                    set_point.Slot);

                            Clients.Add(set_point.Id, inst_point);
                        }
                    }
                    else
                    {
                        foreach (Client client in Clients.Values)
                        {
                            client.Dispose();
                        }
                        Clients.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Title}. Clients reader");
            }

            base.DatabaseReadHandler(state);
        }

        #endregion

    }
}
