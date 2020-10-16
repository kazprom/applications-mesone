using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;

namespace S7_DB_gate
{

    public class Tags : LibDBgate.BaseTableTags
    {

        #region CONSTANTS


        public const string col_name_plc_data_type = "plc_data_type";
        public const string col_name_db_no = "db_no";
        public const string col_name_db_offset = "db_offset";
        public const string col_name_req_type = "req_type";

        #endregion


        #region PROPERTIES

        public DataTable Source { get { return source; } }

        #endregion

        public Tags()
        {
            try
            {
                source.TableName = "tags";


                source.Columns.Add(col_name_plc_data_type, typeof(string)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
                {
                    data_type = OdbcType.VarChar,
                    size = 10
                });
                source.Columns.Add(col_name_db_no, typeof(int)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
                {
                    data_type = OdbcType.Int,
                });
                source.Columns.Add(col_name_db_offset, typeof(int)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
                {
                    data_type = OdbcType.Int,
                });

                source.Columns.Add(col_name_req_type, typeof(string)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
                {
                    data_type = OdbcType.VarChar,
                    size = 15
                });
            }
            catch (Exception ex)
            {

                throw new Exception("Error constructor", ex);
            }


        }
    }
}
