using LibMESone.Tables;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

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

                LibMESone.ConfigFile config_file = new LibMESone.ConfigFile();
                LibMESone.Core core = new LibMESone.Core(config_file, typeof(Service));

                common.InfinityWaiting();

            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
            }
        }
    }
}
