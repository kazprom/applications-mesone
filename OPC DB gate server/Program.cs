using Lib;
using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;

[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("OPC DB Gate Server")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("1.0.*")]


namespace OPC_DB_gate_server
{
    class Program
    {

        private static bool FirstStart = true;
        
        public static Database database = new Database();

        static void Main(string[] args)
        {
            Global.PrintAppInfo();
            Global.Subscribe_Ctrl_C();

            Clients.Subcribe();


            while (true)
            {
                try
                {

#if DEBUG
                    if (FirstStart || DateTime.Now.Second % 5 == 0)
#else
                    if(DateTime.Now.Second == 0)
#endif
                    {
                        database.Type = Settings.XML.file.ReadValue("DB_TYPE", database.Type);
                        database.ConnectionString = Settings.XML.file.ReadValue("CONNECTION_STRING", database.ConnectionString);

                        database.Read(Settings.Database.table.Table);
                        database.Read(Clients.Database.table.Table);
                        database.Read(Tags.Database.table.Table);

                        foreach (TCPconnection conn in Clients.connections)
                        {
                            conn.SendEncrypt(Tags.
                                             Database.
                                             table.
                                             Table.AsEnumerable().
                                             Where(x => x.Field<int>(DBTable.EColumns.id.ToString()) == conn.ID));
                        }

                        Thread.Sleep(1000);
                    }


                    if (FirstStart || DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                    {

                        Logger.DeleteOldFiles(int.Parse(Settings.XML.file.ReadValue("DEPTH_LOG_DAY", "2")));
                        Thread.Sleep(1000);
                    }


                    Thread.Sleep(100);

                }
                catch (Exception ex)
                {
                    Logger.WriteMessage("Error execution", ex);
                    Thread.Sleep(5000);
                }

                FirstStart = false;
            }
        }
    }
}
