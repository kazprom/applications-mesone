using System;
using System.Collections.Generic;
using System.Text;

namespace KingPigeonS272_DB_gate
{
    class CTag : LibPlcDBgate.CTag
    {

        private string channel;
        public string Channel
        {
            get
            {
                return (channel);
            }
            set
            {
                if(!Equals(channel, value))
                {
                    channel = value;
                    Logger.Info($"Channel = {channel}");
                }
            }
        }

    }
}
