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

            Lib.Buffer<OPC_DB_gate_Lib.TagData> buffer = new Lib.Buffer<OPC_DB_gate_Lib.TagData>(10000);

            Settings settings = new Settings();
            Clients clients = new Clients();
            Tags tags = new Tags();
            RT_values rt_values = new RT_values();
            History history = new History();

            BufferHandler buffer_handler = new BufferHandler(buffer, rt_values, history);

            Database database = new Database(config_file.DB_TYPE,
                                             config_file.CONNECTION_STRING,
                                             settings,
                                             clients,
                                             tags,
                                             rt_values,
                                             history);

            Connections connections = new Connections(clients, tags, buffer);

            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}
