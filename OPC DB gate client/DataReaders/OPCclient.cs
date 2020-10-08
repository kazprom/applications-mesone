using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OPC_DB_gate_client
{
    class OPCclient : IDataReader
    {

        #region VARIABLES

        private string name;

        private Dictionary<int, Opc.Da.SubscriptionState> group_states = new Dictionary<int, Opc.Da.SubscriptionState>();

        #endregion


        public OPCclient(string name, Lib.Buffer<OPC_DB_gate_Lib.TagData> buffer) : base(name, buffer)
        {
            try
            {
                this.name = name;

                Opc.URL url = new Opc.URL($"opcda://localhost/{name}");
                Opc.Da.Server server = null;
                OpcCom.Factory fact = new OpcCom.Factory();
                server = new Opc.Da.Server(fact, null);
                server.Connect(url, new Opc.ConnectData(new System.Net.NetworkCredential()));

                PutEvent += GroupHandlerRunner;

            }
            catch (Exception ex)
            {
                throw new Exception("Error create OPC client", ex);
            }

        }


        private void GroupHandlerRunner(Dictionary<int, Group> groups)
        {

            try
            {

                foreach (var item in groups)
                {
                    if (!group_states.ContainsKey(item.Key))
                    {
                        group_states.Add(item.Key, new Opc.Da.SubscriptionState());
                        Lib.Message.Make($"OPC server {name} add group {item.Key}");
                    }

                    Opc.Da.SubscriptionState groupState = group_states[item.Key];
                    groupState.Name = item.Value.Rate.ToString();
                    groupState.UpdateRate = item.Value.Rate;
                    groupState.Active = true;

                    /*
                    Opc.Da.Subscription group = (Opc.Da.Subscription)server.CreateSubscription(groupList[i]);
                    Opc.Da.Item[] items = new Opc.Da.Item[1];

                    for (int j = 0; j < devices.Count; j++)
                    {
                        items[0] = new Opc.Da.Item();
                        items[0].ItemName = groupList[i].Name;
                    }
                    items = group.AddItems(items);

                    group.DataChanged += Group_DataChanged;


                    if (!times.ContainsKey(item.Key))
                    {
                        times.Add(item.Key, new Timer(TimerCallback, item.Value, 0, item.Key));
                    }
                    /**/
                }

                /*
                                foreach (var item in times)
                                {
                                    if (!groups.ContainsKey(item.Key))
                                        item.Value.Dispose();
                                }

                                var itemsToRemove = times.Where(f => f.Value == null).ToArray();
                                foreach (var item in itemsToRemove)
                                    times.Remove(item.Key);
                /**/
            }
            catch (Exception ex)
            {
                throw new Exception("Error group handler runner", ex);
            }



        }



    }
}
