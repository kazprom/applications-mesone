using CsvHelper;
using System;
using System.Collections.Generic;
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
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main()
        {
            try
            {
                Lib.Common common = new Lib.Common();

                string value;
                using (var reader = new StreamReader(@"c:\temp\CSVtest\AB0000007.csv"))
                using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.CurrentCulture))
                {
                    Console.WriteLine(csv.Configuration.Delimiter);
                    Console.WriteLine(csv.Configuration.Quote);
                    Console.WriteLine(csv.Configuration.DetectColumnCountChanges);
                    Console.WriteLine(csv.Configuration.HasHeaderRecord);
                    csv.Configuration.IgnoreQuotes = true;

                    csv.Read();
                    csv.ReadHeader();
                    string[] headerRow = csv.Context.HeaderRecord;

                    csv.Configuration.PrepareHeaderForMatch = (header, index) =>
                    {
                        if (string.IsNullOrWhiteSpace(header))
                        {
                            return $"Blank{index}";
                        }

                        return header;
                    };

                    var records = csv.GetRecords<dynamic>().ToList();

                    Console.ReadKey();


                    /*
                    //csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();

                    foreach (var item in headerRow)
                    {
                        Console.WriteLine(item);
                    }

                    var records = csv.GetRecords<dynamic>() as IDictionary<string, object>;

                    foreach (var item in records)
                    {
                        Console.WriteLine(item.GetType());
                    }

                    */

                    //csv.GetRecords<string>();

                    //csv.ReadHeader();


                    /*
                    csv.Configuration.BadDataFound = x =>
                    {

                        Console.WriteLine($"Bad data: <{x.RawRecord}>");
                        Console.ReadKey();
                    };

                    for (int i = 1; csv.Read(); i++)
                    {
                        Console.Write(i + " ");
                        for (int j = 1; csv.TryGetField<string>(j, out value); j++)
                        {
                            Console.Write(value + " | ");
                        }
                        Console.WriteLine("");
                    }
                    */
                }





                common.InfinityWaiting();

            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
            }
        }




    }
}
