using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OPC_DB_gate_server
{
    public class TCPclients
    {

        #region CONSTANTS

        const string col_name_id = "id";
        const string col_name_ip = "ip";
        const string col_name_port = "port";

        #endregion

        #region VARIABLES

        public DataTable settings = new DataTable("clients");
        private List<Client> clients = new List<Client>();

        #endregion

        #region PROPERTIES


        #endregion

        #region CONSTRUCTOR

        public TCPclients()
        {
            var id_col = settings.Columns.Add(col_name_id, typeof(int));
            settings.Columns.Add(col_name_ip, typeof(string));
            settings.Columns.Add(col_name_port, typeof(int));

            settings.PrimaryKey = new DataColumn[] { id_col };

            settings.RowChanged += Settings_Changed;
            settings.RowDeleted += Remove_Clients;

        }

        #endregion


        #region PUBLICS



        #endregion


        #region PRIVATES

        private void Settings_Changed(object sender, DataRowChangeEventArgs e)
        {
            try
            {

                int id = (int)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(col_name_id)];
                IPAddress ip = IPAddress.Parse((string)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(col_name_ip)]);
                int port = (int)e.Row.ItemArray[e.Row.Table.Columns.IndexOf(col_name_port)];



                switch (e.Action)
                {
                    case DataRowAction.Change:
                        {
                            clients.Find(x => x.ID == id).Settings(ip, port);
                            break;
                        }
                    case DataRowAction.Add:
                        {
                            clients.Add(new Client(id, ip, port));
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error add/change TCP clients", ex);
            }


        }

        private void Remove_Clients(object sender, DataRowChangeEventArgs e)
        {
            try
            {
                switch (e.Action)
                {
                    case DataRowAction.Delete:
                        {
                            List<Client> unnecessary = clients.ToList();
                            foreach (DataRow row in e.Row.Table.Rows)
                            {
                                Client client = unnecessary.Find(x => x.ID == (int)row.ItemArray[e.Row.Table.Columns.IndexOf(col_name_id)]);
                                if (client != null)
                                    unnecessary.Remove(client);
                            }
                            foreach (Client client in unnecessary)
                            {
                                int index = clients.FindIndex(x => x.ID == client.ID);
                                if (index != -1)
                                {
                                    clients[index].Dispose();
                                    clients.RemoveAt(index);
                                }
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error remove TCP clients", ex);
            }
        }

        #endregion

        #region CLASSES

        private class Client : IDisposable
        {


            #region VARIABLES


            private string title;
            private bool disposedValue;
            private bool thread_execution_flag = true;
            private Thread thread_connect;
            private Thread thread_read;
            private Thread thread_write;
            private TcpListener listener;
            private TcpClient client;
            private NetworkStream nwStream = null;
            byte[] buf = new byte[1024];

            #endregion

            #region PROPERTIES

            private int id;
            public int ID { get { return id; } }

            private IPAddress ip;
            public IPAddress IP { get { return ip; } }

            private int port;
            public int Port { get { return port; } }

            #endregion

            #region CONSTRUCTOR

            public Client(int id, IPAddress ip, int port)
            {
                this.id = id;
                Settings(ip, port);
            }

            #endregion

            #region DESTRUCTOR

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        listener.Stop();
                        thread_execution_flag = false;

                        thread_connect.Join();
                        thread_connect = null;
                    }

                    disposedValue = true;
                }
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }

            #endregion

            #region PUBLICS

            public void Settings(IPAddress ip, int port)
            {

                if (!ip.Equals(this.ip) || port != this.port)
                {

                    thread_execution_flag = false;

                    if (thread_connect != null)
                    {
                        thread_connect.Join();
                        thread_connect = null;

                        Logger.WriteMessage($"{title} changed settings to {ip}:{port}. Reconnecting ...");
                    }

                    GC.Collect();

                    this.port = port;
                    this.ip = ip;

                    title = $"Listener ID<{id}> {ip}:{port}";


                    thread_execution_flag = true;

                    thread_connect = new Thread(new ThreadStart(HadlerConnect)) { IsBackground = true, Name = $"Connect ID - {id} Ip - {this.ip}:{this.port}" };
                    thread_connect.Start();

                }
            }

            #endregion

            #region PRIVATES

            private void HadlerConnect()
            {
                try
                {
                    while (thread_execution_flag)
                    {
                        try
                        {
                            listener = new TcpListener(ip, port);
                            listener.Start();
                            Logger.WriteMessage($"{title} opened");

                            using (client = new TcpClient())
                            {
                                while (thread_execution_flag)
                                {
                                    try
                                    {

                                        if (!client.Connected)
                                        {
                                            Logger.WriteMessage($"{title} waiting ...");
                                            if (listener.Pending())
                                                client = listener.AcceptTcpClient();
                                        }
                                        else
                                        {
                                            if (nwStream == null)
                                            {
                                                nwStream = client.GetStream();
                                                Logger.WriteMessage($"{title} connected with client {((IPEndPoint)client.Client.RemoteEndPoint).Address}:{((IPEndPoint)client.Client.RemoteEndPoint).Port}");

                                                thread_read = new Thread(new ThreadStart(HandlerRead)) { IsBackground = true, Name = $"Read ID - {id} Ip - {this.ip}:{this.port}" };
                                                thread_read.Start();

                                                thread_write = new Thread(new ThreadStart(HandlerWrite)) { IsBackground = true, Name = $"Write ID - {id} Ip - {this.ip}:{this.port}" };
                                                thread_write.Start();
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.WriteMessage($"{title} error connection", ex);
                                    }

                                    Thread.Sleep(5000);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteMessage($"{title} error opening", ex);
                        }

                        Thread.Sleep(5000);
                    }

                    if (thread_read != null)
                    {
                        thread_read.Join();
                        thread_read = null;
                    }

                    if (thread_write != null)
                    {
                        thread_write.Join();
                        thread_write = null;
                    }


                    if (nwStream != null)
                    {
                        nwStream.Close();
                        nwStream = null;
                    }

                    if (client != null)
                    {
                        client.Close();
                        client.Dispose();
                        client = null;
                    }

                    if (listener != null)
                    {
                        listener.Server.Close();
                        listener.Stop();
                        listener = null;
                    }

                    GC.Collect();

                    Logger.WriteMessage($"{title} closed");
                }
                catch (Exception ex)
                {
                    Logger.WriteMessage($"{title} error closing", ex);
                }
            }

            private void HandlerRead()
            {

                try
                {
                    while (thread_execution_flag)
                    {
                        int size_package = 0;

                        if (nwStream != null && nwStream.DataAvailable)
                        {
                            size_package = nwStream.Read(buf, 0, buf.Length);


                            if (size_package > 0)
                            {

                                byte[] income_package = new byte[size_package];
                                System.Buffer.BlockCopy(buf, 0, income_package, 0, size_package);
                                /*
                                List<byte[]> packages = Lib.Protocol.CatchPackage(income_package);

                                if (packages != null)
                                {
                                    foreach (byte[] package in packages)
                                    {
                                        byte[] data = package;

                                        switch (Protocol.GetTypePackage(ref data))
                                        {
                                            case Protocol.ePackageTypes.DATA:
                                                try
                                                {
                                                    Global.SCell cell = (Global.SCell)Protocol.ConvertByteArrToObj(Encryption.Decrypt(data));
                                                    Lib.Buffer.Enqueue(cell);
                                                    RealtimeValues.WriteCell(cell);
                                                }
                                                catch (Exception ex)
                                                {
                                                    Logger.WriteMessage($"{title} error getting package with data object", ex);
                                                }
                                                break;
                                        }
                                    }
                                }
                                */
                            }
                        }
                    }

                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {

                    Logger.WriteMessage($"{title} error read data", ex);
                    nwStream = null;

                }
            }

            private void HandlerWrite()
            {

                try
                {
                    while (thread_execution_flag)
                    {
                        if (DateTime.Now.Second % 10 == 0)
                        {

                            if (nwStream != null)
                            {
                                //ping
                                try
                                {
                                    byte[] msg = { 7, 7, 7 };
                                    nwStream.Write(msg, 0, msg.Length);
                                }
                                catch (Exception)
                                {
                                    Logger.WriteMessage($"{title} remote client disconnected");
                                    nwStream = null;
                                    return;
                                }
                                /*
                                //send keys
                                nwStream.Write(Protocol.BuildPackage
                                                (Protocol.ConvertObjToByteArr
                                                    (Encryption.SafetyKeys), Protocol.ePackageTypes.KEY));

                                //send tags
                                nwStream.Write(Protocol.BuildPackage
                                                (Encryption.Encrypt
                                                    (Protocol.ConvertObjToByteArr(Tags.Get(id))), Protocol.ePackageTypes.INFO));
                                */
                            }

                            Thread.Sleep(1000);
                        }

                        Thread.Sleep(100);

                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteMessage($"{title} error write data", ex);
                    nwStream = null;
                }
            }

            #endregion

        }

        #endregion

    }
}
