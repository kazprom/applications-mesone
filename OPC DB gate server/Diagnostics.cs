using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace OPC_DB_gate_server
{
    public class Diagnostics
    {

        #region CONSTANTS

        public const string col_name_id = "id";
        public const string col_name_clients_id = "clients_id";
        public const string col_name_state = "state";
        public const string col_name_clock = "clock";
        public const string col_name_appinfo = "appinfo";
        public const string col_name_message = "message";

        #endregion


        #region PROPERTIES

        private DataTable source = new DataTable("diagnostics");
        public DataTable Source { get { return source; } }

        #endregion


        public Diagnostics()
        {
            try
            {
                source.Columns.Add(col_name_id, typeof(int)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp() { data_type = System.Data.Odbc.OdbcType.BigInt, ignore = true });
                source.Columns.Add(col_name_clients_id, typeof(int)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp() { data_type = System.Data.Odbc.OdbcType.BigInt, primary_key = true });
                source.Columns.Add(col_name_state, typeof(string)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp() { data_type = System.Data.Odbc.OdbcType.VarChar });
                source.Columns.Add(col_name_clock, typeof(DateTime)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp() { data_type = System.Data.Odbc.OdbcType.DateTime });
                source.Columns.Add(col_name_appinfo, typeof(string)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp() { data_type = System.Data.Odbc.OdbcType.VarChar });
                source.Columns.Add(col_name_message, typeof(string)).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp() { data_type = System.Data.Odbc.OdbcType.VarChar });
            }
            catch (Exception ex)
            {
                throw new Exception("Error constructor", ex);
            }
        }


        #region PUBLICS

        public void Put(long id, OPC_DB_gate_Lib.ClientInfo info)
        {
            try
            {
                DataRow row = GetRow(id);

                row[col_name_clock] = info.clock;
                row[col_name_appinfo] = info.appinfo;
                row[col_name_message] = info.message;

            }
            catch (Exception ex)
            {
                throw new Exception("Error put", ex);
            }

        }


        public void PutState(long id, string state)
        {
            DataRow row = GetRow(id);

            row[col_name_state] = state;
        }


        #endregion


        #region PRIVATES

        private DataRow GetRow(long id)
        {
            lock (source)
            {
                DataRow row = source.Select($"{col_name_clients_id} = {id}").FirstOrDefault();

                if (row == null)
                {
                    row = source.NewRow();
                    row[col_name_clients_id] = id;
                    source.Rows.Add(row);
                }

                return row;
            }
        }


        #endregion

    }
}
