using LibMESone;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("KingPigeonS272 DB Gate")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: Guid("809A63E0-5A83-4F2F-99D2-2F9C3E58BFE5")]

[assembly: AssemblyVersion("1.0.*")]

namespace KingPigeonS272_DB_gate
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
