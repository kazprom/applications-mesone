using LibMESone;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ModBUS DB Gate")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: Guid("2C46A19D-063E-44F8-AF0D-554D75060EE7")]

[assembly: AssemblyVersion("1.0.*")]

namespace ModBUS_DB_gate
{
    class Program
    {

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            try
            {
                Lib.Common common = new Lib.Common();

                CConfigFile config_file = new CConfigFile();
                CCORE<CCUSTOM> core = new CCORE<CCUSTOM>();

                config_file.ReadCompleted += (CConfigFile sender) => { core.LoadSettingFromConfigFile(sender); };

                common.InfinityWaiting();

            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
            }
        }
    }
}
