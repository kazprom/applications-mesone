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
        static void Main()
        {
            Lib.Global.PrintAppInfo();
            Lib.Global.Subscribe_Ctrl_C();

            Lib.Console console = new Lib.Console();
            Lib.TextLogger text_logger = new Lib.TextLogger();

            LibMESone.ConfigFile config_file = new LibMESone.ConfigFile();



            Lib.Global.InfinityWaiting();
        }
    }
}
