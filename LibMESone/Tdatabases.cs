using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Text;

namespace LibMESone
{
    class Tdatabases: TbaseNE
    {

        #region CONSTANTS

        public const string col_name_database = "database";
        public readonly static Type data_type_database = typeof(string);

        public const string col_name_driver = "driver";
        public readonly static Type data_type_driver = typeof(string);

        public const string col_name_hosts_id = "hosts_id";
        public readonly static Type data_type_hosts_id = typeof(long);

        public const string col_name_port = "port";
        public readonly static Type data_type_port = typeof(int);

        public const string col_name_username = "username";
        public readonly static Type data_type_username = typeof(string);

        public const string col_name_password = "password";
        public readonly static Type data_type_password = typeof(string);

        #endregion



        public Tdatabases()
        {

            source.TableName = "databases";

            source.Columns.Add(col_name_database, data_type_database).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
            {
                data_type = OdbcType.VarChar,
                size = 255
            });

            source.Columns.Add(col_name_driver, data_type_driver).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
            {
                data_type = OdbcType.VarChar,
                size = 255
            });

            source.Columns.Add(col_name_hosts_id, data_type_hosts_id).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
            {
                data_type = OdbcType.BigInt
            });

            source.Columns.Add(col_name_port, data_type_port).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
            {
                data_type = OdbcType.Int
            });

            source.Columns.Add(col_name_username, data_type_username).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
            {
                data_type = OdbcType.VarChar,
                size = 255
            });

            source.Columns.Add(col_name_password, data_type_password).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
            {
                data_type = OdbcType.VarChar,
                size = 255
            });

        }


    }
}
