using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Text;

namespace LibMESone.Tables
{
    public class Thosts:TbaseNE
    {

        #region CONSTANTS

        public const string col_name_IPaddress = "IPaddress";
        public Lib.Database.SExtProp prop_IPaddress = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.VarChar,
            size = 15
        };

        public const string col_name_DNSname = "DNSname";
        public Lib.Database.SExtProp prop_DNSname = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.VarChar,
            size = 255
        };

        #endregion



        public Thosts()
        {

            source.TableName = "hosts";

            source.Columns.Add(col_name_IPaddress, typeof(string)).ExtendedProperties.Add(prop_IPaddress.GetType(), prop_IPaddress);
            source.Columns.Add(col_name_DNSname, typeof(string)).ExtendedProperties.Add(prop_DNSname.GetType(), prop_DNSname);

        }

    }
}
