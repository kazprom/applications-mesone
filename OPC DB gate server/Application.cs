﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace OPC_DB_gate_server
{
    class Application
    {

        #region CONSTANTS

        public const string col_name_id = "id";
        public const string col_name_key = "key";
        public const string col_name_value = "value";

        #endregion


        #region ENUMS

        public enum EKeys
        {
            APPINFO,
            CLOCK

        }

        #endregion


        #region PROPERTIES

        private DataTable source = new DataTable("application");
        public DataTable Source { get { return source; } }

        #endregion

        public Application()
        {
            try
            {

                source.Columns.Add(col_name_id, typeof(int));
                source.Columns.Add(col_name_key, typeof(string)).ExtendedProperties.Add(typeof(Lib.CDatabase.SExtProp), new Lib.CDatabase.SExtProp() {  data_type = System.Data.Odbc.OdbcType.VarChar,  primary_key = true });
                source.Columns.Add(col_name_value, typeof(string)).ExtendedProperties.Add(typeof(Lib.CDatabase.SExtProp), new Lib.CDatabase.SExtProp() { data_type = System.Data.Odbc.OdbcType.VarChar});

            }
            catch (Exception ex)
            {

                throw new Exception("Error constructor", ex);
            }

        }


        public void Put(EKeys key, string value)
        {
            try
            {
                DataRow row;

                lock (source)
                {
                    row = source.Select($"{col_name_key} = '{key}'").FirstOrDefault();

                    if (row == null)
                    {
                        row = source.NewRow();
                        row[col_name_key] = key.ToString();
                        source.Rows.Add(row);
                    }
                }

                row[col_name_value] = value;
            }
            catch (Exception ex)
            {
                throw new Exception("Error put", ex);
            }
        }

    }
}
