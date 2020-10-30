using System;
using System.Data;
using System.Linq;

namespace S7_DB_gate.Tables
{
    public class Tdiagnostics: LibMESone.Tables.BaseID
    {
        /*
        igtb yi
        #region CONSTANTS

        public const string col_name_clients_id = "clients_id";
        public Lib.Database.SExtProp prop_clients_id = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.BigInt,
            primary_key = true
        };

        public const string col_name_state = "state";
        public Lib.Database.SExtProp prop_state = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.VarChar
        };

        public const string col_name_message = "message";
        public Lib.Database.SExtProp prop_message = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.VarChar
        };


        #endregion


        public Tdiagnostics()
        {
            try
            {

                source.TableName = "diagnostics";

                Lib.Database.SExtProp prop = (Lib.Database.SExtProp)source.Columns[LibMESone.Tables.Tbase.col_name_id].ExtendedProperties[typeof(Lib.Database.SExtProp)];
                prop.ignore = true;

                source.Columns.Add(col_name_clients_id, typeof(int)).ExtendedProperties.Add(prop_clients_id.GetType(), prop_clients_id);
                source.Columns.Add(col_name_state, typeof(string)).ExtendedProperties.Add(prop_state.GetType(), prop_state);
                source.Columns.Add(col_name_message, typeof(string)).ExtendedProperties.Add(prop_message.GetType(), prop_message);

            }
            catch (Exception ex)
            {
                throw new Exception("Error constructor", ex);
            }
        }


        #region PUBLICS

        public void Put(long id)
        {
            try
            {
                DataRow row = GetRow(id);

                row[col_name_state] = "";
                row[col_name_message] = "";

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

        */
    }
}