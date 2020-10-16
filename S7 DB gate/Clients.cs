using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace S7_DB_gate
{
    public class Clients:LibDBgate.BaseTableNDE
    {


        #region CONSTANTS

        public const string col_name_cpu_type = "cpu_type";
        public const string col_name_ip = "ip";
        public const string col_name_port = "port";
        public const string col_name_rack = "rack";
        public const string col_name_slot = "slot";

        #endregion

        #region PROPERTIES

        public DataTable Source { get { return source; } }

        #endregion


        public Clients()
        {
            try
            {

                source.TableName = "clients";

                source.Columns.Add(col_name_cpu_type, typeof(string)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
                {
                    data_type = System.Data.Odbc.OdbcType.VarChar,
                    size = 10
                });
                source.Columns.Add(col_name_ip, typeof(string)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
                {
                    data_type = System.Data.Odbc.OdbcType.VarChar,
                    size = 15
                });
                source.Columns.Add(col_name_port, typeof(int)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
                {
                    data_type = System.Data.Odbc.OdbcType.SmallInt
                });
                source.Columns.Add(col_name_rack, typeof(short)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
                {
                    data_type = System.Data.Odbc.OdbcType.SmallInt
                });
                source.Columns.Add(col_name_slot, typeof(short)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
                {
                    data_type = System.Data.Odbc.OdbcType.SmallInt
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error constructor", ex);
            }
        }

    }
}
