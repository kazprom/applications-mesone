using System.Data;
using System.Data.Odbc;

namespace LibDBgate.Tables
{
    public class Ttags : LibMESone.Tables.TbaseNE
    {

        public new class Row : LibMESone.Tables.TbaseNE.Row
        {
            public long clients_id;
            public int rate;
            public TagData.EDataType data_type;
            public bool rt_value_enabled;
            public bool history_enabled;

            public static void DataRowToRow(DataRow row, ref Row r_row)
            {
                LibMESone.Tables.TbaseNE.Row b_row = r_row;
                LibMESone.Tables.TbaseNE.Row.DataRowToRow(row, ref b_row);
                r_row.clients_id = (long)row[col_name_clients_id];
                r_row.rate = (int)row[col_name_rate];
                r_row.data_type = (TagData.EDataType)(byte)row[col_name_data_type];
                r_row.history_enabled = (bool)row[col_name_history_enabled];
                r_row.enabled = (bool)row[col_name_enabled];
            }
        }

        #region CONSTANTS

        public const string col_name_clients_id = "clients_id";
        public Lib.Database.SExtProp prop_clients_id = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.BigInt
        };

        public const string col_name_rate = "rate";
        public Lib.Database.SExtProp prop_rate = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.SmallInt
        };

        public const string col_name_data_type = "data_type";
        public Lib.Database.SExtProp prop_data_type = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.TinyInt
        };

        public const string col_name_rt_values_enabled = "rt_values_enabled";
        public Lib.Database.SExtProp prop_rt_values_enabled = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.TinyInt
        };

        public const string col_name_history_enabled = "history_enabled";
        public Lib.Database.SExtProp prop_history_enabled = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.TinyInt
        };

        #endregion

        public Ttags()
        {
            source.Columns.Add(col_name_clients_id, typeof(long)).ExtendedProperties.Add(prop_clients_id.GetType(), prop_clients_id);
            source.Columns.Add(col_name_rate, typeof(int)).ExtendedProperties.Add(prop_rate.GetType(), prop_rate);
            source.Columns.Add(col_name_data_type, typeof(byte)).ExtendedProperties.Add(prop_data_type.GetType(), prop_data_type);
            source.Columns.Add(col_name_rt_values_enabled, typeof(bool)).ExtendedProperties.Add(prop_rt_values_enabled.GetType(), prop_rt_values_enabled);
            source.Columns.Add(col_name_history_enabled, typeof(bool)).ExtendedProperties.Add(prop_history_enabled.GetType(), prop_history_enabled);
        }
    }
}
