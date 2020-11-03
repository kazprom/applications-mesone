using Lib;
using System;
using System.Reflection;
using System.Threading;


[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("OPC DB Gate Client")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("1.0.*")]

namespace OPC_DB_gate_client
{
    class Program
    {
        static void Main(string[] args)
        {

            Lib.Common.PrintAppInfo();
            Lib.Common.Subscribe_Ctrl_C();

            Lib.Console console = new Lib.Console();

            ConfigFile config_file;

            if (args.Length == 1)
            {
                config_file = new ConfigFile(args[0]);
            }
            else
            {
                config_file = new ConfigFile(Lib.Common.NameExeFile.Split('.')[0] + ".xml");
            }

            LibOPCDBgate.ClientInfo info = new LibOPCDBgate.ClientInfo() { appinfo = Lib.Common.AppInfo() };
            Lib.Buffer<LibDBgate.TagData> buffer = new Lib.Buffer<LibDBgate.TagData>(10000);

            TCPconnection connection = new TCPconnection(config_file.SERVER_IP,
                                                         config_file.SERVER_PORT,
                                                         buffer,
                                                         info);



            DataReaders data_readers = new DataReaders(connection, buffer);


            while (true)
            {

                info.clock = DateTime.Now;

                Thread.Sleep(1000);
            }


        }
    }
}
