using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper;
using LibMESone;

namespace CSV_DB_gate
{
    public class CConverter : LibDBgate.CSrvSUB
    {

        #region PROPERTIES

        public Structs.CSetConverter Settings { get; set; }

        #endregion


        /*
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

                  */
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
        public override void LoadSetting(ISetting setting)
        {
            Settings = setting as Structs.CSetConverter;

            CycleRate = Settings.Frequency_sec * 250;

            foreach (var item in Settings.Fields)
            {
                Parent.Logger.Debug($" {item.NameSource} {item.DataType} {item.NameDestination}");
            }


            //throw new NotImplementedException();
        }

        public override void Timer_Handler(object state)
        {




            //Parent.Logger.Debug("timer handler");

            //throw new NotImplementedException();
        }
    }
}
