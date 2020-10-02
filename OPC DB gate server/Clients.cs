using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace OPC_DB_gate_server
{
    static class Clients
    {

        public static class Database
        {
            public static DBTable table = new DBTable("clients");

            public enum EColumns
            {
                ip,
                port
            }

            static Database()
            {
                table.AddColumn(EColumns.ip.ToString(), typeof(string));
                table.AddColumn(EColumns.port.ToString(), typeof(int));
            }

        }


        #region VARIABLES

        public static List<TCPconnection> connections = new List<TCPconnection>();

        #endregion




        public static void Subcribe()
        {

            Database.table.Table.RowChanged += Change_Connections;
            Database.table.Table.RowDeleting += Change_Connections;


        }


        #region PRIVATES

        private static void Change_Connections(object sender, DataRowChangeEventArgs e)
        {
            try
            {

                int id = (int)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(DBTable.EColumns.id.ToString())];
                IPAddress ip = IPAddress.Parse((string)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(Database.EColumns.ip.ToString())]);
                int port = (int)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(Database.EColumns.port.ToString())];


                switch (e.Action)
                {
                    case DataRowAction.Add:
                        {
                            connections.Add(new TCPconnection(id, ip, port));
                            break;
                        }
                    case DataRowAction.Change:
                        {
                            connections.Find(x => x.ID == id).Settings(ip, port);
                            break;
                        }
                    case DataRowAction.Delete:
                        {
                            int index = connections.FindIndex(x => x.ID == id);
                            connections[index].Dispose();
                            connections.RemoveAt(index);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error change TCP connection", ex);
            }


        }

        #endregion


    }
}
