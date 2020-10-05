using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OPC_DB_gate_server
{




    public class Tags
    {

        #region CONSTANTS

        public const string col_name_clients_id = "clients_id";
        public const string col_name_path = "path";
        public const string col_name_rate = "rate";
        public const string col_name_data_type = "data_type";

        #endregion


        #region PROPERTIES

        private DBTable source = new DBTable("tags");
        public DBTable Source { get { return source; } }


        private Dictionary<int, Dictionary<int, OPC_DB_gate_Lib.TagSettings>> groups = new Dictionary<int, Dictionary<int, OPC_DB_gate_Lib.TagSettings>>();
        public Dictionary<int, Dictionary<int, OPC_DB_gate_Lib.TagSettings>> Groups { get { return groups; } }

        #endregion


        public Tags()
        {

            source.AddColumn(col_name_clients_id, typeof(int));
            source.AddColumn(col_name_path, typeof(string));
            source.AddColumn(col_name_rate, typeof(int));
            source.AddColumn(col_name_data_type, typeof(byte));

            source.Table.RowChanged += TagGrouping;
            source.Table.RowDeleting += TagGrouping;

        }



        #region PRIVATES


        private void TagGrouping(object sender, System.Data.DataRowChangeEventArgs e)
        {

            try
            {
                int id = (int)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(DBTable.EColumns.id.ToString())];
                int clients_id = (int)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(col_name_clients_id)];
                string path = (string)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(col_name_path)];
                int rate = (int)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(col_name_rate)];
                OPC_DB_gate_Lib.TagSettings.EDataType data_type = (OPC_DB_gate_Lib.TagSettings.EDataType)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(col_name_data_type)];


                lock (groups)
                {

                    if (!groups.ContainsKey(clients_id))
                        groups.Add(clients_id, new Dictionary<int, OPC_DB_gate_Lib.TagSettings>());

                    switch (e.Action)
                    {
                        case DataRowAction.Add:
                            {
                                groups[clients_id].Add(id, new OPC_DB_gate_Lib.TagSettings() { path = path, rate = rate, data_type = data_type });
                                break;
                            }
                        case DataRowAction.Change:
                            {
                                OPC_DB_gate_Lib.TagSettings ts = groups[clients_id][id];
                                ts.path = path;
                                ts.rate = rate;
                                ts.data_type = data_type;
                                break;
                            }
                        case DataRowAction.Delete:
                            {
                                groups[clients_id].Remove(id);
                                if (groups[clients_id].Count == 0)
                                    groups.Remove(clients_id);
                                break;
                            }
                    }
                }

            }
            catch (Exception ex)
            {

                throw new Exception("Error tags grouping", ex);
            }

        }

        #endregion
    }
}
