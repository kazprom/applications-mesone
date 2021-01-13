using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace CSV_DB_gate
{
    class CCUSTOM : LibDBgate.CCUSTOM
    {


        #region CONSTANTS

        private const string KEY_BASE_PATH = "BASE_PATH";
        private const string KEY_HIS_PATH = "HIS_PATH";

        #endregion

        #region PROPERTIES

        public IEnumerable<Tables.CConverter> TConverters { get; set; }

        public IEnumerable<Tables.CField> TFields { get; set; }

        #endregion


        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {

            try
            {

                if (DB != null && TSettings != null)
                {

                    string base_path = TSettings.Where(x => x.Key.Equals(KEY_BASE_PATH, StringComparison.OrdinalIgnoreCase)).Select(x => x.Value).FirstOrDefault();
                    if (base_path == null)
                    {
                        Logger.Warn($"Can't find parameter {KEY_BASE_PATH}");
                    }

                    string his_path = TSettings.Where(x => x.Key.Equals(KEY_HIS_PATH, StringComparison.OrdinalIgnoreCase)).Select(x => x.Value).FirstOrDefault();
                    if (his_path == null)
                    {
                        Logger.Warn($"Can't find parameter {KEY_HIS_PATH}");
                    }

                    switch (DB.CheckTable<Tables.CConverter>(Tables.CConverter.TableName))
                    {
                        case true:
                            TConverters = DB.WhereRead<Tables.CConverter>(Tables.CConverter.TableName, new { Enabled = true });
                            break;
                    }


                    switch (DB.CheckTable<Tables.CField>(Tables.CField.TableName))
                    {
                        case true:
                            TFields = DB.WhereRead<Tables.CField>(Tables.CField.TableName, new { Enabled = true });
                            break;
                    }


                    if (TConverters != null && TFields != null)
                    {
                        var data = from convs in TConverters
                                   where convs.Enabled == true
                                   select new
                                   {
                                       Parent = this,
                                       convs.Id,
                                       convs.Name,
                                       Base_path = base_path,
                                       convs.File_path,
                                       convs.File_delete,
                                       His_path = his_path,
                                       File_depth_his = convs.File_depth_his,
                                       Table_clear = convs.Table_clear,
                                       Start_timestamp = convs.Start_timestamp,
                                       Frequency_sec = convs.Frequency_sec,
                                       Timeout_sec = convs.Timeout_sec,
                                       Has_header_record = convs.Has_header_record,
                                       Delimiter = convs.Delimiter,
                                       Quote = convs.Quote,
                                       Quotes_ignore = convs.Quotes_ignore,
                                       Detect_column_count_changes = convs.Detect_column_count_changes,
                                       Replaceable = convs.Replaceable,

                                       Fields = from fields in TFields
                                                where fields.Enabled == true && fields.Converters_id == convs.Id
                                                select new
                                                {
                                                    NameSource = fields.Name_src,
                                                    NameDestination = fields.Name_dst,
                                                    DataType = Lib.Field.TypeParce(fields.Data_type),
                                                    fields.Unique
                                                }
                                   };


                        Dictionary<ulong, Dictionary<string, object>> children_props = data.ToDictionary(o => o.Id,
                                                                                                             o => o.
                                                                                                                  GetType().
                                                                                                                  GetProperties().ToDictionary(z => z.Name,
                                                                                                                                               z => z.GetValue(o)));

                        CUD<CConverter>(children_props);

                    }


                }


            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            base.Timer_Handler(sender, e);
        }


    }
}
