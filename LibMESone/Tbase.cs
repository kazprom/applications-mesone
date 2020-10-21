using System;
using System.Data;
using System.Data.Odbc;

namespace LibMESone
{
    public class Tbase
    {
        #region CONSTANTS

        public const string col_name_id = "id";
        public readonly static Type data_type_id = typeof(long);

        #endregion

        protected DataTable source = new DataTable();


        public Lib.Database.SExtProp prop_id = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.BigInt,
            primary_key = true,
            auto_increment = true,
            not_null = true
        };


        public Tbase()
        {

            source.Columns.Add(col_name_id, data_type_id).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), prop_id);

        }

    }
}
