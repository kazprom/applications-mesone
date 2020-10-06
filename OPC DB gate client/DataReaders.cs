using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPC_DB_gate_client
{
    public class DataReaders
    {

        #region VARIABLES

        private TCPconnection connection;
        Lib.Buffer<OPC_DB_gate_Lib.TagData> buffer;
        private Dictionary<string, IDataReader> data_readers = new Dictionary<string, IDataReader>();

        #endregion

        #region CONSTRUCTOR

        public DataReaders(TCPconnection connection, Lib.Buffer<OPC_DB_gate_Lib.TagData> buffer)
        {

            this.buffer = buffer;
            this.connection = connection;
            this.connection.GetTags += Connection_GetTags;

        }

        #endregion

        #region PRIVATES

        private void Connection_GetTags(Dictionary<int, OPC_DB_gate_Lib.TagSettings> value)
        {
            try
            {
                Dictionary<string, List<IDataReader.STag>> tag_groups = new Dictionary<string, List<IDataReader.STag>>();

                foreach (var item in value)
                {
                    string[] str_parts = item.Value.path.Split('\\');
                    if (str_parts.Length == 2)
                    {
                        string reader_name = str_parts[0];
                        string tag_path = str_parts[1];

                        if (!tag_groups.ContainsKey(reader_name))
                            tag_groups.Add(reader_name, new List<IDataReader.STag>());

                        tag_groups[reader_name].Add(new IDataReader.STag() { id = item.Key, path = tag_path, rate = item.Value.rate, data_type = item.Value.data_type });
                    }
                }

                foreach (var item in data_readers)
                {
                    if (!tag_groups.ContainsKey(item.Key))
                        item.Value.Dispose();
                }

                var itemsToRemove = data_readers.Where(f => f.Value.IsDisposed == true).ToArray();
                foreach (var item in itemsToRemove)
                    data_readers.Remove(item.Key);

                foreach (var item in tag_groups)
                {
                    if (!data_readers.ContainsKey(item.Key))
                    {
                        if (item.Key == IntInfo.default_name)
                            data_readers.Add(item.Key, new IntInfo(buffer));
                        else
                            data_readers.Add(item.Key, new OPCclient(item.Key, buffer));

                    }
                    data_readers[item.Key].Put(item.Value);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error get tags", ex);
            }

        }

        #endregion


    }
}
