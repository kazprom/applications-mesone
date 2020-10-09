using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPC_DB_gate_client
{
    class IntInfo : IDataReader
    {


        #region CONSTANTS

        public const string default_name = "Int.Info";
        private const string Folder_clock = "Clock";
        private const string Tag_seconds = "Seconds";
        private const string Tag_minutes = "Minutes";
        private const string Tag_hours = "Hours";

        #endregion


        private Dictionary<int, Timer> times = new Dictionary<int, Timer>();


        public IntInfo(Lib.Buffer<OPC_DB_gate_Lib.TagData> buffer) : base(default_name, buffer)
        {

            PutEvent += GroupHandlerRunner;

        }

        private void GroupHandlerRunner(Dictionary<int, Group> groups)
        {
            try
            {
                foreach (var item in groups)
                {
                    if (!times.ContainsKey(item.Key))
                    {
                        times.Add(item.Key, new Timer(TimerCallback, item.Value, 0, item.Key));
                        Lib.Message.Make($"Data source [{default_name}] added group [{item.Key}]");
                    }

                }


                foreach (var item in times)
                {
                    if (!groups.ContainsKey(item.Key))
                        item.Value.Dispose();
                }

                var itemsToRemove = times.Where(f => f.Value == null).ToArray();
                foreach (var item in itemsToRemove)
                {
                    times.Remove(item.Key);
                    Lib.Message.Make($"Data source [{default_name}] removed group [{item.Key}]");
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error group handler runner", ex);
            }
        }

        private void TimerCallback(object state)
        {
            Group group = state as Group;
            var tags = group.tags;
            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    string path = tag.path.ToLower();
                    object result = null;

                    if (path == $"{Folder_clock}.{Tag_seconds}".ToLower())
                        result = DateTime.Now.Second;

                    if (path == $"{Folder_clock}.{Tag_minutes}".ToLower())
                        result = DateTime.Now.Minute;

                    if (path == $"{Folder_clock}.{Tag_hours}".ToLower())
                        result = DateTime.Now.Hour;

                    buffer.Enqueue(new OPC_DB_gate_Lib.TagData()
                    {
                        id = tag.id,
                        quality = result != null ? OPC_DB_gate_Lib.TagData.EQuality.Good : OPC_DB_gate_Lib.TagData.EQuality.Bad,
                        timestamp = DateTime.Now,
                        value = OPC_DB_gate_Lib.TagSettings.ObjToDataType(result, tag.data_type)
                    });


                }
            }
        }










        /*


        private void Handler()
        {
            while (execution)
            {

                try
                {
                    foreach (var group in groups)
                    {
                        


                    }


                }
                catch (Exception ex)
                {
                    Logger.WriteMessage($"{Name} execution error", ex);
                }

                Thread.Sleep(10);

            }


        }
        */

    }
}
