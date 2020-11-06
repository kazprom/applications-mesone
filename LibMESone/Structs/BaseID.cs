using Lib;

namespace LibMESone.Structs
{
    public class BaseID: Table
    {

        [Field(Field.Etype.BigInt, pk: true, ai: true, nn: true)]
        public long Id { get; set; }




        /*
        public class Row
        {

            public long id;

            public static void DataRowToRow(DataRow row, ref Row r_row)
            {
                r_row.id = (long)row[col_name_id];
            }

        }

        #region CONSTANTS

        public const string col_name_id = "id";
        public Lib.Database.SExtProp prop_id = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.BigInt,
            primary_key = true,
            auto_increment = true,
            not_null = true
        };

        #endregion


        #region PROPERTIES

        protected DataTable source = new DataTable();
        public DataTable Source { get { return source; } }

        #endregion


        #region CONSTRUCTOR
        public Tbase()
        {
            source.Columns.Add(col_name_id, typeof(long)).ExtendedProperties.Add(prop_id.GetType(), prop_id);
        }
        #endregion
        */
    }
}
