using System;
using System.Linq;
using System.Reflection;

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

        public static Lib.Config config = new Lib.Config();


        static void Main(string[] args)
        {
            Lib.Global.PrintInfo();

            if (args.Count() >= 1)
            {
                config.PathFile = args[0];
            }
            else
            {
                config.PathFile = $"{Lib.Global.NameExeFile.Split('.')[0]}.xml";
            }

            config.Nodes.Add(new Lib.Config.SNode() { path = "DB_TYPE", default_value = "MySQL" });
            config.Nodes.Add(new Lib.Config.SNode() { path = "CONNECTION_STRING", default_value = "Driver={mySQL ODBC 8.0 ANSI Driver}; Server=myServerAddress;Option=131072;Stmt=;Database=myDataBase;User=myUsername;Password=myPassword;" });
            config.Nodes.Add(new Lib.Config.SNode() { path = "DEPTH_LOG_DAY", default_value = "2" });


            Lib.Global.Wait_Ctrl_C();

        }




    }
}
