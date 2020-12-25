using LibMESone;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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
        private static NLog.Logger logger = NLog.LogManager.GetLogger("Main");

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
