using Lib;
using LibMESone.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;

namespace S7_DB_gate
{
    public class CCUSTOM : LibPlcDBgate.CCUSTOM
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
                if (DB != null)
                {
                    switch (DB.CheckTable<Tables.CClient>(Tables.CClient.TableName))
                    {
                        case true:
                            TClients = DB.WhereRead<Tables.CClient>(Tables.CClient.TableName, new { Enabled = true });
                            break;
                    }


                    switch (DB.CheckTable<Tables.CTag>(Tables.CTag.TableName))
                    {
                        case true:
                            TTags = DB.WhereRead<Tables.CTag>(Tables.CTag.TableName, new { Enabled = true });
                            break;
                    }




                    if (TClients != null && TTags != null)
                    {
                        var data = from clients in TClients
                                   where clients.Enabled == true
                                   select new
                                   {
                                       Parent = this,
                                       clients.Id,
                                       clients.Name,
                                       clients.Ip,
                                       clients.Port,
                                       clients.Cpu_type,
                                       clients.Rack,
                                       clients.Slot,

                                       Tags = from tags in TTags
                                              where tags.Clients_id == clients.Id && tags.Enabled == true
                                              select new
                                              {
                                                  tags.Id,
                                                  tags.Name,

                                                  tags.Rate,

                                                  tags.Data_type,
                                                  tags.History_enabled,
                                                  tags.RT_values_enabled,

                                                  tags.PLC_data_type,
                                                  tags.Data_block_no,
                                                  tags.Data_block_offset,
                                                  tags.Bit_offset,
                                                  tags.Request_type
                                              }
                                   };

                        /*
                        Tags = from tags in TTags
                                                where tags.Clients_id == clients.Id && tags.Enabled == true
                                                group tags by tags.Rate into groups
                                                select new
                                                {
                                                    Id = groups.Key,
                                                    Name = $"g{groups.Key}",

                                                    Tags = from group_tags in groups
                                                           select new
                                                           {
                                                               Id = group_tags.Id,
                                                               Name = group_tags.Name,
                                                               
                                                               Data_type = group_tags.Data_type,
                                                               History_enabled = group_tags.History_enabled,
                                                               RT_values_enabled = group_tags.RT_values_enabled,

                                                               PLC_data_type = group_tags.PLC_data_type,
                                                               Data_block_no = group_tags.Data_block_no,
                                                               Data_block_offset = group_tags.Data_block_offset,
                                                               Bit_offset = group_tags.Bit_offset,
                                                               Request_type = group_tags.Request_type
                                                           }
                                                }
                        /**/

                        Dictionary<ulong, Dictionary<string, object>> children_props = data.ToDictionary(o => o.Id,
                                                                                                         o => o.
                                                                                                              GetType().
                                                                                                              GetProperties().ToDictionary(z => z.Name,
                                                                                                                                           z => z.GetValue(o)));

                        CUD<CClient>(children_props);
                    }

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
