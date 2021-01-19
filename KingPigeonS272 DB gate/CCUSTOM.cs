using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Linq;

namespace KingPigeonS272_DB_gate
{
    class CCUSTOM: LibPlcDBgate.CCUSTOM
    {

        #region PROPERTIES

        public IEnumerable<Tables.CSockets> TSockets { get; set; }

        public IEnumerable<Tables.CClient> TClients { get; set; }
        
        public IEnumerable<Tables.CTag> TTags { get; set; }

        #endregion



        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {

            try
            {

                if (DB != null)
                {
                    switch (DB.CheckTable<Tables.CSockets>(Tables.CSockets.TableName))
                    {
                        case true:
                            TSockets = DB.WhereRead<Tables.CSockets>(Tables.CSockets.TableName, new { Enabled = true });
                            break;
                    }


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




                    if (TSockets != null && TClients != null && TTags != null)
                    {

                        
                        var data = from sockets in TSockets
                                   where sockets.Enabled == true
                                   select new
                                   {
                                       Parent = this,
                                       sockets.Id,
                                       sockets.Name,
                                       sockets.Ip,
                                       sockets.Port,

                                       Clients = from clients in TClients
                                              where clients.Sockets_id == sockets.Id && clients.Enabled == true
                                              select new
                                              {
                                                  clients.Id,
                                                  clients.Name,
                                                  clients.Imei,
                                                  clients.Timeout_m,

                                                  Tags = from tags in TTags
                                                         where tags.Clients_id == clients.Id && tags.Enabled == true
                                                         select new
                                                         {
                                                             tags.Id,
                                                             tags.Name,

                                                             tags.Channel,

                                                             tags.Rate,

                                                             tags.Data_type,
                                                             tags.RT_values_enabled,
                                                             tags.History_enabled
                                                         }
                                              }
                                   };



                        Dictionary<ulong, Dictionary<string, object>> children_props = data.ToDictionary(o => o.Id,
                                                                                                         o => o.
                                                                                                              GetType().
                                                                                                              GetProperties().ToDictionary(z => z.Name,
                                                                                                                                           z => z.GetValue(o)));

                        CUD<CSocket>(children_props);

                    }

                }


            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


            base.Timer_Handler(sender, e);
        }

    }
}
