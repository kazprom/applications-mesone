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
            Lib.Global.PrintAppInfo();
            Lib.Global.Subscribe_Ctrl_C();

            ConfigFile config_file;

            if (args.Length == 1)
            {
                config_file = new ConfigFile(args[0]);
            }
            else
            {
                config_file = new ConfigFile(Lib.Global.NameExeFile.Split('.')[0] + ".xml");
            }

            TCPconnection connection = new TCPconnection(config_file.SERVER_IP,
                                                         config_file.SERVER_PORT);

            DataReaders data_readers = new DataReaders(connection.Tags);


            while (true)
            {
                Thread.Sleep(100);
            }


        }
    }
}
