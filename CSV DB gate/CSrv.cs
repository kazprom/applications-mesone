using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace CSV_DB_gate
{
    public class CSrv : LibDBgate.CSrvCUSTOM
    {

        #region CONSTANTS

        private const string KEY_BASE_PATH = "BASE_PATH";
        private const string KEY_HIS_PATH = "HIS_PATH";

        #endregion

        #region PROPERTIES

        public IEnumerable<Tables.CConverter> TConverters { get; set; }

        public IEnumerable<Tables.CField> TFields { get; set; }

        #endregion

        #region PUBLICS

        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {


            try
            {
                if (Database != null && Settings != null)
                {

                    string base_path = Settings.Where(x => x.Key.Equals(KEY_BASE_PATH, StringComparison.OrdinalIgnoreCase)).Select(x => x.Value).FirstOrDefault();
                    if (base_path == null)
                    {
                        Logger.Warn($"Can't find parameter {KEY_BASE_PATH}");
                        return;
                    }

                    string his_path = Settings.Where(x => x.Key.Equals(KEY_HIS_PATH, StringComparison.OrdinalIgnoreCase)).Select(x => x.Value).FirstOrDefault();
                    if (his_path == null)
                    {
                        Logger.Warn($"Can't find parameter {KEY_HIS_PATH}");
                        return;
                    }

                    switch (Database.CheckExistTable(Tables.CConverter.TableName))
                    {
                        case null:
                        case false:
                            return;
                    }

                    switch (Database.CompareTableSchema<Tables.CConverter>(Tables.CConverter.TableName))
                    {
                        case null:
                        case false:
                            return;
                    }

                    TConverters = Database.WhereRead<Tables.CConverter>(Tables.CConverter.TableName, new { Enabled = true });

                    switch (Database.CheckExistTable(Tables.CField.TableName))
                    {
                        case null:
                        case false:
                            return;
                    }

                    switch (Database.CompareTableSchema<Tables.CField>(Tables.CField.TableName))
                    {
                        case null:
                        case false:
                            return;
                    }

                    TFields = Database.WhereRead<Tables.CField>(Tables.CField.TableName, new { Enabled = true });

                    IEnumerable<Structs.CSetConverter> settings = default;

                    if (TConverters != null && TFields != null)
                    {
                        settings = from convs in TConverters
                                   where convs.Enabled == true
                                   select new Structs.CSetConverter()
                                   {
                                       Id = convs.Id,
                                       Name = convs.Name,
                                       Base_path = base_path,
                                       File_path = convs.File_path,
                                       File_delete = convs.File_delete,
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
                                                select new Structs.CField()
                                                {
                                                    NameSource = fields.Name_src,
                                                    NameDestination = fields.Name_dst,
                                                    DataType = Lib.Field.TypeParce(fields.Data_type),
                                                    Unique = fields.Unique
                                                }
                                   };
                    }

                    CUD<CConverter>(settings);

                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            base.Timer_Handler(sender, e);


        }

        #endregion

    }
}
