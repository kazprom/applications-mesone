using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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



        public override void LoadSetting(ISetting setting)
        {
            try
            {
                Settings = setting as Structs.CSetConverter;

                if (Settings != null)
                {
                    Logger = NLog.LogManager.GetLogger($"{Parent.Logger.Name} Task [{Settings.Id}] <{Settings.Name}>");
                    CycleRate = Settings.Frequency_sec * 250;

                }
                else
                {
                    Logger = NLog.LogManager.GetCurrentClassLogger();
                    CycleRate = 0;
                }

            }
            catch (Exception ex)
            {
                if (Logger != null) Logger.Error(ex);
            }

        }

        public override void Timer_Handler(object state)
        {
            try
            {

                DataTable source = ReadFile();

                DataTable target = PrepareTable(source);

                WriteTable(target);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }


        private DataTable ReadFile()
        {
            DataTable result = null;

            try
            {

                if (!File.Exists(Settings.File_path))
                {
                    Logger.Warn($"File {Settings.File_path} is absent");
                    return null;
                }

                using (var reader = new StreamReader(Settings.File_path))
                using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.CurrentCulture))
                {
                    csv.Configuration.HasHeaderRecord = Settings.Has_header_record;
                    csv.Configuration.Delimiter = Settings.Delimiter;
                    csv.Configuration.Quote = Settings.Quote;
                    csv.Configuration.IgnoreQuotes = Settings.Quotes_ignore;
                    csv.Configuration.DetectColumnCountChanges = Settings.Detect_column_count_changes;

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

                    csv.Configuration.BadDataFound = (context) =>
                    {
                        Logger.Warn($"Bad data: <{context.RawRecord}>");
                    };

                    csv.Configuration.MissingFieldFound = (headerNames, index, context) =>
                    {
                        Logger.Warn($"MissingField: [{context.CurrentIndex}] <{context.RawRecord}");
                    };

                    result = new DataTable();
                    result.Columns.AddRange(headerRow.Distinct().Where(x => x != null && x != "").Select(x => new DataColumn(x)).ToArray());


                    while (csv.Read())
                    {
                        DataRow row = result.NewRow();
                        foreach (DataColumn col in result.Columns)
                        {
                            row[col.ColumnName] = csv.GetField(col.ColumnName);
                        }
                        result.Rows.Add(row);
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return result;


        }

        private DataTable PrepareTable(DataTable source)
        {
            DataTable result = null;
            try
            {
                if (source != null)
                {

                    result = new DataTable();

                    result.Columns.AddRange(Settings.
                                            Fields.
                                            GroupBy(x => x.NameDestination).
                                            Select(x => x.First()).
                                            Where(x => x.DataType != null).
                                            Select(x => new DataColumn(x.NameDestination)).
                                            ToArray());

                    string[] source_columns = Settings.
                                              Fields.
                                              GroupBy(x => x.NameSource).
                                              Select(x => x.First()).
                                              Where(x => x.DataType != null).
                                              Select(x => x.NameSource).
                                              ToArray();


                    foreach (DataRow row in source.Rows)
                    {
                        DataRow r = result.NewRow();
                        foreach (var col in source_columns)
                        {

                            var c = row.Table.Columns[col];
                            if (c != null)
                            {
                                string target_column = Settings.
                                                       Fields.
                                                       Where(x => x.NameSource.Equals(col) && x.DataType != null).
                                                       Select(x => x.NameDestination).
                                                       FirstOrDefault();

                                if (target_column != null)
                                {
                                    r[target_column] = row[c.Ordinal];
                                }
                            }
                        }
                        result.Rows.Add(r);
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return result;
        }

        private void WriteTable(DataTable data)
        {
            try
            {
                if (data != null)
                {

                    CSrv parrent = Parent as CSrv;

                    Action add_table = () =>
                        {
                            Dictionary<string, Lib.Field> table_struct = Settings.Fields.
                                                                            GroupBy(x => x.NameDestination).Select(x => x.First()).
                                                                            Where(x => x.DataType != null).
                                                                            ToDictionary(
                                                                                        x => x.NameDestination,
                                                                                        x => new Lib.Field()
                                                                                        {
                                                                                            TYPE = (Lib.Field.EDoctrine)x.DataType
                                                                                        });

                            if (table_struct.ContainsKey("id"))
                                table_struct.Remove("id");

                            if (table_struct.ContainsKey("created_at"))
                                table_struct.Remove("created_at");

                            if (table_struct.ContainsKey("updated_at"))
                                table_struct.Remove("updated_at");


                            table_struct.Add("id", new Lib.Field() { TYPE = Lib.Field.EDoctrine.BigInt, PK = true, AI = true });
                            table_struct.Add("created_at", new Lib.Field() { TYPE = Lib.Field.EDoctrine.DateTime });
                            table_struct.Add("updated_at", new Lib.Field() { TYPE = Lib.Field.EDoctrine.DateTime });

                            parrent.Database.CreateTable($"{Tables.CTargetTable.TablePrefix}{Settings.Name}", table_struct);
                        };

                    switch (parrent.Database.CheckExistTable($"{Tables.CTargetTable.TablePrefix}{Settings.Name}"))
                    {
                        case true:
                            parrent.Database.RemoveTable($"{Tables.CTargetTable.TablePrefix}{Settings.Name}");
                            add_table();
                            break;

                        case false:
                            add_table();
                            break;
                    }

                    foreach (DataRow row in data.Rows)
                    {
                        parrent.Database.Insert($"{Tables.CTargetTable.TablePrefix}{Settings.Name}",
                                                (IReadOnlyDictionary<string, object>)row.ItemArray.
                                                Select((value, index) => new { value, index }).
                                                Where(x => x.value != DBNull.Value).
                                                ToDictionary(x => row.Table.Columns[x.index].ColumnName, y => y.value));
                    }


                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
