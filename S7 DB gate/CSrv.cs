using Lib;
using LibMESone.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;

namespace S7_DB_gate
{
    public class CSrv : LibPlcDBgate.CSrv
    {
        #region PROPERTIES

        public IEnumerable<Tables.CClient> TClients { get; set; }

        public IEnumerable<Tables.CTag> TTags { get; set; }

        #endregion


        #region PUBLICS

        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {

            try
            {
                if (Database != null && Settings != null)
                {
                    switch (Database.CheckTable<Tables.CClient>(Tables.CClient.TableName))
                    {
                        case null:
                        case false:
                            return;
                    }

                    TClients = Database.WhereRead<Tables.CClient>(Tables.CClient.TableName, new { Enabled = true });

                    switch (Database.CheckTable<Tables.CTag>(Tables.CTag.TableName))
                    {
                        case null:
                        case false:
                            return;
                    }

                    TTags = Database.WhereRead<Tables.CTag>(Tables.CTag.TableName, new { Enabled = true });

                    IEnumerable<Structs.CClient> settings = default;

                    if (TClients != null && TTags != null)
                    {
                        settings = from clients in TClients
                                   where clients.Enabled == true
                                   select new Structs.CClient()
                                   {
                                       Id = clients.Id,
                                       Name = clients.Name,
                                       Cpu_type = clients.Cpu_type,
                                       Ip = clients.Ip,
                                       Port = clients.Port,
                                       Rack = clients.Rack,
                                       Slot = clients.Slot,

                                       Groups = from tags in TTags
                                                where tags.Clients_id == clients.Id && tags.Enabled == true
                                                group tags by tags.Rate into groups
                                                select new LibPlcDBgate.Structs.CGroup()
                                                {
                                                    Rate = groups.Key,

                                                    Tags = from group_tags in groups
                                                           select new Structs.CTag()
                                                           {
                                                               PLC_data_type = group_tags.PLC_data_type,
                                                               Data_block_no = group_tags.Data_block_no,
                                                               Data_block_offset = group_tags.Data_block_offset,
                                                               Bit_offset = group_tags.Bit_offset,
                                                               Request_type = group_tags.Request_type,
                                                               Data_type = group_tags.Data_type,
                                                               History_enabled = group_tags.History_enabled,
                                                               RT_values_enabled = group_tags.RT_values_enabled
                                                           }
                                                }
                                   };
                    }

                    CUD<CClient>(settings);

                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            base.Timer_Handler(sender, e);
        }

        #endregion

    }
}
