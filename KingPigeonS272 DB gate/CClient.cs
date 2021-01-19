using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace KingPigeonS272_DB_gate
{
    class CClient : LibMESone.CSrvCyc
    {

        #region VARIABLES

        private DateTime ts;

        #endregion


        #region PROPERTIES

        private string imei;
        public string Imei
        {
            get
            {
                return (imei);
            }
            set
            {
                if (!Equals(imei, value))
                {
                    imei = value;
                    Logger.Info($"IMEI = {imei}");
                }
            }
        }

        private ushort timeout_m;
        public ushort Timeout_m
        {
            get
            {
                return(timeout_m);
            }
            set
            {
                if(timeout_m != value)
                {
                    timeout_m = value;
                    Logger.Info($"Timeout = {timeout_m}");
                }
            }
        }

        public dynamic Tags
        {
            set
            {
                try
                {

                    var data = from tags in (IEnumerable<dynamic>)value
                               group tags by tags.Rate into groups
                               select new
                               {
                                   Parent = this,
                                   Id = groups.Key,

                                   Tags = from group_tags in groups
                                          select new
                                          {
                                              group_tags.Id,
                                              group_tags.Name,

                                              group_tags.Channel,

                                              group_tags.Data_type,
                                              group_tags.History_enabled,
                                              group_tags.RT_values_enabled

                                          }
                               };



                    Dictionary<ulong, Dictionary<string, object>> children_props = data.ToDictionary(o => (ulong)o.Id,
                                                                                                     o => o.
                                                                                                          GetType().
                                                                                                          GetProperties().ToDictionary(z => z.Name,
                                                                                                                                       z => z.GetValue(o)));
                    CUD<CGroup>(children_props);


                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        #endregion



        public CClient()
        {
            try
            {
                CycleRate = 20000;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }



        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {

            try
            {



            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            base.Timer_Handler(sender, e);
        }


    }
}
