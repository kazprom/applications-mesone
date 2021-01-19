using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KingPigeonS272_DB_gate
{
    class CGroup : LibPlcDBgate.CGroup
    {


        public override dynamic Tags
        {
            set
            {
                try
                {
                    base.Tags = value;

                    var data = from tags in (IEnumerable<dynamic>)value
                               select new
                               {
                                   Parent = this,

                                   tags.Id,
                                   tags.Name,

                                   tags.Channel,

                                   tags.Data_type,
                                   tags.History_enabled,
                                   tags.RT_values_enabled

                               };



                    Dictionary<ulong, Dictionary<string, object>> children_props = data.ToDictionary(o => (ulong)o.Id,
                                                                                                     o => o.
                                                                                                          GetType().
                                                                                                          GetProperties().ToDictionary(z => z.Name,
                                                                                                                                       z => z.GetValue(o)));
                    CUD<CTag>(children_props);



                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

            }
        }


    }
}
