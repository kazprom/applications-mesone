using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Text;

namespace LibMESone.Tables
{
    public class Tdatabases: TbaseNE
    {

        #region CONSTANTS

        public const string col_name_database = "database";
        public Lib.Database.SExtProp prop_database = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.VarChar,
            size = 255
        };

        public const string col_name_driver = "driver";
        public Lib.Database.SExtProp prop_driver = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.VarChar,
            size = 255
        };

        public const string col_name_hosts_id = "hosts_id";
        public Lib.Database.SExtProp prop_hosts_id = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.BigInt
        };

        public const string col_name_port = "port";
        public Lib.Database.SExtProp prop_port = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.Int
        };

        public const string col_name_username = "username";
        public Lib.Database.SExtProp prop_username = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.VarChar,
            size = 255
        };

        public const string col_name_password = "password";
        public Lib.Database.SExtProp prop_password = new Lib.Database.SExtProp()
        {
            data_type = OdbcType.VarChar,
            size = 255
        };

        #endregion



        public Tdatabases()
        {

            source.TableName = "databases";

            source.Columns.Add(col_name_database, typeof(string)).ExtendedProperties.Add(prop_database.GetType(), prop_database);
            source.Columns.Add(col_name_driver, typeof(string)).ExtendedProperties.Add(prop_driver.GetType(), prop_driver);
            source.Columns.Add(col_name_hosts_id, typeof(long)).ExtendedProperties.Add(col_name_hosts_id.GetType(), col_name_hosts_id);
            source.Columns.Add(col_name_port, typeof(int)).ExtendedProperties.Add(prop_port.GetType(), prop_port);
            source.Columns.Add(col_name_username, typeof(string)).ExtendedProperties.Add(prop_username.GetType(), prop_username);
            source.Columns.Add(col_name_password, typeof(string)).ExtendedProperties.Add(prop_password.GetType(), prop_password);

        }


    }
}
