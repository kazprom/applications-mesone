using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using CsvHelper;
using LibMESone;

namespace CSV_DB_gate
{
    public class CConverter : LibDBgate.CSrvSUB
    {

        #region CONSTANTS

        private const string COL_NAME_ID = "id";
        private const string COL_NAME_CREATED_AT = "created_at";
        private const string COL_NAME_UPDATED_AT = "updated_at";

        #endregion

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
                    CycleRate = 200;
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

        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {
            try
            {
                var time = DateTime.Now.Subtract(Settings.Start_timestamp).TotalSeconds % Settings.Frequency_sec;

                if (time < 0 || time > 1)
                {
                    //Console.WriteLine(time);
                }
                else
                {
                    Thread.Sleep(1000);

                    string file_path = Settings.Base_path + Path.DirectorySeparatorChar + Settings.File_path;

                    if (!File.Exists(file_path))
                    {
                        Logger.Warn($"File {Settings.File_path} is absent");
                    }
                    else
                    {



                        DataTable source = ReadFile(file_path);

                        DataTable target = PrepareTable(source);

                        WriteTable(target);


                        string path_dest = $"{Settings.His_path}" +
                                      $"{Path.DirectorySeparatorChar}" +
                                      $"{Name}";

                        string file_name = Path.GetFileName(file_path);

                        //Save history files
                        if (Settings.File_depth_his > 0)
                        {
                            try
                            {

                                DirectoryInfo di = Directory.CreateDirectory(path_dest);

                                File.Copy(file_path, path_dest +
                                                     $"{Path.DirectorySeparatorChar}" +
                                                     $"{DateTime.Now:yyyy_MM_dd_HH_mm_ss}_{file_name}");

                            }
                            catch (Exception ex)
                            {
                                Logger.Warn(ex, "Can't save history file");
                            }
                        }

                        //Clear history
                        string[] filePaths = Directory.GetFiles(path_dest, $"*{file_name}");
                        Array.Sort(filePaths, StringComparer.InvariantCulture);
                        Array.Reverse(filePaths);
                        for (uint i = Settings.File_depth_his; i < filePaths.Length; i++)
                        {
                            File.Delete(filePaths[i]);
                        }

                        //Delete original file
                        if (Settings.File_delete)
                        {
                            try
                            {
                                File.Delete(file_path);
                                Logger.Info($"File {file_path} deleted");
                            }
                            catch (Exception ex)
                            {
                                Logger.Warn(ex, "Can't delete file");
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            base.Timer_Handler(sender, e);
        }


        private DataTable ReadFile(string file_path)
        {
            DataTable result = null;

            try
            {
                using (var reader = new StreamReader(file_path))
                using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.CurrentCulture))
                {

                    Logger.Info($"Read {file_path}");

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
                        Logger.Warn($"Bad data: R[{context.Row}] <{context.RawRecord}>");
                    };

                    csv.Configuration.MissingFieldFound = (headerNames, index, context) =>
                    {
                        Logger.Warn($"MissingField: R[{context.Row}] <{context.RawRecord}");
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

                    using (result = new DataTable())
                    {
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
                        return result;
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

                    CSrv parent = Parent as CSrv;

                    string table_name = $"{Tables.CTargetTable.TablePrefix}{Settings.Name}";

                    Dictionary<string, Lib.Field> table_struct = Settings.Fields.
                                                                    GroupBy(x => x.NameDestination).Select(x => x.First()).
                                                                    Where(x => x.DataType != null).
                                                                    ToDictionary(
                                                                                x => x.NameDestination,
                                                                                x => new Lib.Field()
                                                                                {
                                                                                    TYPE = (Lib.Field.EDoctrine)x.DataType,
                                                                                    UQ = x.Unique
                                                                                });

                    if (table_struct.ContainsKey(COL_NAME_ID))
                        table_struct.Remove(COL_NAME_ID);

                    if (table_struct.ContainsKey(COL_NAME_CREATED_AT))
                        table_struct.Remove(COL_NAME_CREATED_AT);

                    if (table_struct.ContainsKey(COL_NAME_UPDATED_AT))
                        table_struct.Remove(COL_NAME_UPDATED_AT);


                    table_struct.Add(COL_NAME_ID, new Lib.Field() { TYPE = Lib.Field.EDoctrine.BigInt, PK = true, AI = true, NN = true });
                    table_struct.Add(COL_NAME_CREATED_AT, new Lib.Field() { TYPE = Lib.Field.EDoctrine.DateTime });
                    table_struct.Add(COL_NAME_UPDATED_AT, new Lib.Field() { TYPE = Lib.Field.EDoctrine.DateTime });


                    switch (parent.Database.CheckExistTable(table_name))
                    {
                        case true:

                            switch (parent.Database.CompareTableSchema(table_name, table_struct))
                            {
                                case false:
                                    parent.Database.RemoveTable(table_name);
                                    parent.Database.CreateTable(table_name, table_struct);
                                    break;

                                case true:
                                    if (Settings.Table_clear)
                                    {
                                        parent.Database.ClearTable(table_name);
                                    }
                                    break;
                            }

                            break;

                        case false:
                            parent.Database.CreateTable(table_name, table_struct);
                            break;
                    }

                    foreach (DataRow row in data.Rows)
                    {

                        Dictionary<string, object> new_row = row.ItemArray.
                                                                Select((value, index) => new { value, index }).
                                                                Where(x => x.value != DBNull.Value).
                                                                ToDictionary(x => row.Table.Columns[x.index].ColumnName, y => y.value);


                        Dictionary<string, object> constraints = new_row.Where(x => table_struct.ContainsKey(x.Key) && table_struct[x.Key].UQ).ToDictionary(x => x.Key, x => x.Value);
                        Dictionary<string, object> values = new_row.Where(x => table_struct.ContainsKey(x.Key) && !table_struct[x.Key].UQ).ToDictionary(x => x.Key, x => x.Value);

                        values.Add(COL_NAME_UPDATED_AT, DateTime.Now);
                        if (!Settings.Replaceable || !parent.Database.Update(table_name, constraints, values, false))
                        {
                            values.Remove(COL_NAME_UPDATED_AT);
                            new_row.Add(COL_NAME_CREATED_AT, DateTime.Now);
                            parent.Database.Insert(table_name, new_row);
                        }
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
