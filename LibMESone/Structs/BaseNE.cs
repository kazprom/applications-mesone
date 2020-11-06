using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;
using System.Xml;

namespace LibMESone.Structs
{
    public class BaseNE : BaseID
    {
        [Field(Field.Etype.VarChar, 255)]
        public string Name { get; set; }

        [Field(Field.Etype.TinyInt, 1)]
        public bool Enabled { get; set; }

        /*

        public new class Row : Tbase.Row
        {
            public string name;
            public bool enabled;

            public static void DataRowToRow(DataRow row, ref Row r_row)
            {
                Tbase.Row b_row = r_row;
                Tbase.Row.DataRowToRow(row, ref b_row);
                r_row.name = (string)row[col_name_name];
                r_row.enabled = (bool)row[col_name_enabled];
            }
        }


        #region CONSTANTS

        public const string col_name_name = "name";
        public Lib.Database.SExtProp prop_name = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.VarChar,
            size = 255
        };

        public const string col_name_enabled = "enabled";
        public Lib.Database.SExtProp prop_enabled = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.TinyInt
        };

        #endregion

        public TbaseNE()
        {
            source.Columns.Add(col_name_name, typeof(string)).ExtendedProperties.Add(prop_name.GetType(), prop_name);
            source.Columns.Add(col_name_enabled, typeof(bool)).ExtendedProperties.Add(prop_enabled.GetType(), prop_enabled);
        }


        */
    }
}
