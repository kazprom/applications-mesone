using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSV_DB_gate
{
    public class CSrv : LibDBgate.CSrvCUSTOM
    {

        #region PROPERTIES

        public IEnumerable<Tables.CConverter> TConverters { get; set; }

        public IEnumerable<Tables.CField> TFields { get; set; }

        #endregion

        #region PUBLICS

        public override void Timer_Handler(object state)
        {

            base.Timer_Handler(state);

            try
            {

                if (Database != null)
                {

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
                                       File_path = convs.File_path,
                                       File_delete = convs.File_delete,
                                       Table_clear = convs.Table_clear,
                                       Start_timestamp = convs.Start_timestamp,
                                       Frequency_sec = convs.Frequency_sec,
                                       Timeout_sec = convs.Timeout_sec,
                                       Has_header_record = convs.Has_header_record,
                                       Delimiter = convs.Delimiter,
                                       Quote = convs.Quote,
                                       Quotes_ignore = convs.Quotes_ignore,
                                       Detect_column_count_changes = convs.Detect_column_count_changes,

                                       Fields = from fields in TFields
                                                where fields.Enabled == true && fields.Converters_id == convs.Id
                                                select new Structs.CField()
                                                {
                                                    NameSource = fields.Name_src,
                                                    NameDestination = fields.Name_dst,
                                                    DataType = Lib.Field.TypeParce(fields.Data_type)
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
        }

        #endregion

    }
}
