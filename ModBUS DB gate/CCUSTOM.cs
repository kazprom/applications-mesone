using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace ModBUS_DB_gate
{
    class CCUSTOM : LibPlcDBgate.CCUSTOM
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
                                       clients.Protocol,
                                       clients.Address,

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

                                                  tags.Function,
                                                  tags.Address
                                              }
                                   };

                       

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
