using System.Data;
using System.Data.Odbc;

namespace Lib
{
    public class BaseTable
    {

        protected DataTable source = new DataTable();

        public const string col_name_id = "id";
        public Database.SExtProp prop_id = new Database.SExtProp()
        {
            data_type = OdbcType.BigInt,
            primary_key = true,
            auto_increment = true,
            not_null = true
        };


        public BaseTable()
        {

            source.Columns.Add(col_name_id, typeof(int)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), prop_id);

        }

    }
}
