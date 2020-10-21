using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;

namespace LibMESone
{
    public class TbaseNE : Tbase
    {
        #region CONSTANTS

        public const string col_name_name = "name";
        public readonly static Type data_type_name = typeof(string);

        public const string col_name_enabled = "enabled";
        public readonly static Type data_type_enabled = typeof(bool);

        #endregion

        public TbaseNE()
        {

            source.Columns.Add(col_name_name, data_type_name).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
            {
                data_type = OdbcType.VarChar,
                size = 255
            });

            source.Columns.Add(col_name_enabled, data_type_enabled).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
            {
                data_type = OdbcType.TinyInt,
            });


        }

    }
}
