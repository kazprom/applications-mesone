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

        Opc.Da.Server opc_server = null;
        private Dictionary<int, Opc.Da.SubscriptionState> subscription_states = new Dictionary<int, Opc.Da.SubscriptionState>();
        private Dictionary<int, Opc.Da.ISubscription> subscriptions = new Dictionary<int, Opc.Da.ISubscription>();

        #endregion


        public OPCclient(string name, Lib.Buffer<OPC_DB_gate_Lib.TagData> buffer) : base(name, buffer)
        {
            try
            {
                this.name = name;

                Opc.URL url = new Opc.URL($"opcda://localhost/{name}");
                OpcCom.Factory fact = new OpcCom.Factory();
                opc_server = new Opc.Da.Server(fact, null);
                opc_server.Connect(url, new Opc.ConnectData(new System.Net.NetworkCredential()));

                PutEvent += TagHandler;

            }
            catch (Exception ex)
            {
                throw new Exception("Error create OPC client", ex);
            }

        }


        private void TagHandler(Dictionary<int, Group> groups)
        {

            try
            {

                foreach (var group in groups)
                {
                    Opc.Da.SubscriptionState subscription_state;
                    Opc.Da.ISubscription isubscription;
                    Opc.Da.Subscription subscription;

                    if (!subscription_states.ContainsKey(group.Key))
                    {

                        subscription_state = new Opc.Da.SubscriptionState();
                        subscription_state.Name = group.Key.ToString();
                        subscription_state.ServerHandle = null;
                        subscription_state.ClientHandle = Guid.NewGuid().ToString();
                        subscription_state.Active = true;
                        subscription_state.UpdateRate = group.Value.Rate;
                        subscription_state.Deadband = 0;
                        subscription_state.Locale = null;
                        subscription_states.Add(group.Key, subscription_state);
                        isubscription = opc_server.CreateSubscription(subscription_state);
                        subscriptions.Add(group.Key, isubscription);
                        isubscription.DataChanged += DataChangeHandler;
                        Lib.Message.Make($"OPC server [{opc_server.Name}] added group [{subscription_state.Name}]");
                    }

                    subscription_state = subscription_states[group.Key];
                    subscription = (Opc.Da.Subscription)subscriptions[group.Key];

                    List<Opc.Da.Item> items = new List<Opc.Da.Item>();
                    foreach (var tag in group.Value.tags)
                    {
                        Opc.Da.Item opc_item = subscription.Items.Where(x => x.ItemName == tag.path).FirstOrDefault();
                        if (opc_item == null)
                        {
                            Opc.Da.Item item = new Opc.Da.Item();
                            item.ClientHandle = tag.id;
                            item.ReqType = OPC_DB_gate_Lib.TagSettings.DataTypeToType(tag.data_type);
                            item.ItemName = tag.path;
                            items.Add(item);
                            Lib.Message.Make($"OPC server [{opc_server.Name}] group [{subscription_state.Name}] added tag [{tag.path}]");
                        }
                    }
                    subscription.AddItems(items.ToArray());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error group handler runner", ex);
            }
        }

        #region PRIVATES

        private void DataChangeHandler(object subscriptionHandle, object requestHandle, Opc.Da.ItemValueResult[] values)
        {
            try
            {
                foreach (var item in values)
                {
                    buffer.Enqueue(new OPC_DB_gate_Lib.TagData()
                    {
                        id = (long)item.ClientHandle,
                        timestamp = DateTime.Now,
                        value = item.Value,
                        quality = (OPC_DB_gate_Lib.TagData.EQuality)item.Quality.GetCode()

                    });
                }
            }
            catch (Exception ex)
            {
                Lib.Message.Make("Error data change handler", ex);
            }

        }


        #endregion


    }
}
