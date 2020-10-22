using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Text;

namespace LibMESone.Tables
{
    public class Thosts:TbaseNE
    {

        #region CONSTANTS

        public const string col_name_IPaddress = "database";
        public readonly static Type data_type_IPaddress = typeof(string);

        #endregion



        public Thosts()
        {

            source.TableName = "hosts";

            source.Columns.Add(col_name_IPaddress, data_type_IPaddress).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
            {
                data_type = OdbcType.VarChar,
                size = 15
            });
        }

    }
}
