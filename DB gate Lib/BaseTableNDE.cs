using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;

namespace LibDBgate
{
    public class BaseTableNDE:Lib.BaseTable
    {
        public const string col_name_name = "name";
        //public const string col_name_description = "description";
        public const string col_name_enabled = "enabled";

        public BaseTableNDE()
        {

            source.Columns.Add(col_name_name, typeof(string)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
            {
                data_type = OdbcType.VarChar,
                size = 255
            });
            /*
            source.Columns.Add(col_name_description, typeof(string)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
            {
                data_type = OdbcType.Text,
            });
            */
            source.Columns.Add(col_name_enabled, typeof(bool)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
            {
                data_type = OdbcType.TinyInt,
            });


        }

    }
}
