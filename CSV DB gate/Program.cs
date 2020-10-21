using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("CSV DB Gate")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: Guid("73671BAC-8507-466A-A46E-38EADB04B4CF")]

[assembly: AssemblyVersion("1.0.*")]

namespace CSV_DB_gate
{
    class Program
    {
        static void Main(string[] args)
        {
            Lib.Global.PrintAppInfo();
            Lib.Global.Subscribe_Ctrl_C();

            Lib.Console console = new Lib.Console();
            Lib.TextLogger text_logger = new Lib.TextLogger($@"{Lib.Global.PathExeFolder}LOG");

            HandlerConfigFile config_file = new HandlerConfigFile(Lib.Global.NameExeFile.Split('.')[0] + ".xml");



            Lib.Global.InfinityWaiting();
        }
    }
}
