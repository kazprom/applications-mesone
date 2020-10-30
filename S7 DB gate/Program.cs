using LibMESone.Tables;
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

        private static LibMESone.Core core;
        private static Dictionary<long, Service> services = new Dictionary<long, Service>();

        static void Main()
        {
            try
            {

                Lib.Global.PrintAppInfo();
                Lib.Global.Subscribe_Ctrl_C();

                Lib.Console console = new Lib.Console();
                //LibMESone.Loggers.TextLogger text_logger = new LibMESone.Loggers.TextLogger();
                LibMESone.ConfigFile config_file = new LibMESone.ConfigFile();

                core = new LibMESone.Core(config_file);
                //core.ReadCompleted += ServicesHandler;

                //LibMESone.Loggers.TextLogCleaner text_log_cleaner = new LibMESone.Loggers.TextLogCleaner(text_logger, config_file.DEPTH_LOG_DAY);


                Lib.Global.InfinityWaiting();

            }
            catch (Exception ex)
            {
                Lib.Message.Make("Error main", ex);
                System.Console.ReadKey();
            }
        }


        /*
        private static void ServicesHandler()
        {
            try
            {
                DataRow[] fresh_copies = core.GetServicesByGUID(Lib.Global.AppGUID());

                if(fresh_copies != null)
                {

                    long[] old_ids = services.Keys.ToArray();
                    long[] new_ids = fresh_copies.Select(x => (long)x[Tservices.col_name_id]).ToArray();

                    long[] excess_ids = old_ids.Except(new_ids).ToArray();
                    long[] modify_ids = old_ids.Intersect(new_ids).ToArray();
                    long[] missing_ids = new_ids.Except(old_ids).ToArray();

                    foreach (long id in excess_ids)
                    {
                        services[id].Dispose();
                        services.Remove(id);
                    }

                    foreach (long id in modify_ids)
                    {
                        services[id].UpdateSettings(core.GetServiceDataByID(id));
                    }

                    foreach (long id in missing_ids)
                    {
                        services.Add(id, new Service(core.GetServiceSettingsByID(id)));
                    }
                }
            }
            catch (Exception ex)
            {
                Lib.Message.Make("Can't get copy of services", ex);
            }

        }

        */
    }
}
