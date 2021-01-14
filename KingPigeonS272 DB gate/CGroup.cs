using System;
using System.Collections.Generic;
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

                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

            }
        }


    }
}
