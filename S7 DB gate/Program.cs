using LibMESone;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("S7 DB Gate")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: Guid("4913BEB7-4481-417B-8E5E-D1B05DA4FB3A")]

[assembly: AssemblyVersion("1.0.*")]

namespace S7_DB_gate
{
    class Program
    {

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main()
        {
            try
            {
                Lib.Common common = new Lib.Common();

                CConfigFile config_file = new CConfigFile();
                CCORE<CSrv> core = new CCORE<CSrv>();

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
