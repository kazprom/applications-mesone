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

            Settings settings = new Settings();
            Tags tags = new Tags();
            Clients clients = new Clients(tags.Groups);


            Database database = new Database(config_file.DB_TYPE,
                                             config_file.CONNECTION_STRING,
                                             settings.Source,
                                             clients.Source,
                                             tags.Source);





            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}
