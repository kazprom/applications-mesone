using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace S7_DB_gate.Tables
{
    public class Tclients:LibMESone.Tables.BaseNE
    {
        /*
        public new class Row : LibMESone.Tables.TbaseNE.Row
        {

            public S7.Net.CpuType cpu_type;
            public string ip;
            public int port;
            public short rack;
            public short slot;


            public static void DataRowToRow(DataRow row, ref Row r_row)
            {
                LibMESone.Tables.TbaseNE.Row b_row = r_row;
                LibMESone.Tables.TbaseNE.Row.DataRowToRow(row, ref b_row);

                r_row.cpu_type = (S7.Net.CpuType)Enum.Parse(typeof(S7.Net.CpuType), (string)row[Tclients.col_name_cpu_type]);
                r_row.ip = (string)row[Tclients.col_name_ip];
                r_row.port = (int)row[Tclients.col_name_port];
                r_row.rack = (short)row[Tclients.col_name_rack];
                r_row.slot = (short)row[Tclients.col_name_slot];

            }
        }



        #region CONSTANTS

        public const string col_name_cpu_type = "cpu_type";
        public Lib.Database.SExtProp prop_cpu_type = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.VarChar,
            size = 10
        };

        public const string col_name_ip = "ip";
        public Lib.Database.SExtProp prop_ip = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.VarChar,
            size = 15
        };

        public const string col_name_port = "port";
        public Lib.Database.SExtProp prop_port = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.SmallInt
        };

        public const string col_name_rack = "rack";
        public Lib.Database.SExtProp prop_rack = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.SmallInt
        };

        public const string col_name_slot = "slot";
        public Lib.Database.SExtProp prop_slot = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.SmallInt
        };


        #endregion


        public Tclients()
        {
            try
            {

                source.TableName = "clients";

                source.Columns.Add(col_name_cpu_type, typeof(string)).ExtendedProperties.Add(prop_cpu_type.GetType(),prop_cpu_type);
                source.Columns.Add(col_name_ip, typeof(string)).ExtendedProperties.Add(prop_ip.GetType(), prop_ip);
                source.Columns.Add(col_name_port, typeof(int)).ExtendedProperties.Add(prop_port.GetType(), prop_port);
                source.Columns.Add(col_name_rack, typeof(short)).ExtendedProperties.Add(prop_rack.GetType(), prop_rack);
                source.Columns.Add(col_name_slot, typeof(short)).ExtendedProperties.Add(prop_slot.GetType(), prop_slot);

            }
            catch (Exception ex)
            {
                throw new Exception("Error constructor", ex);
            }
        }


        */
    }
}
