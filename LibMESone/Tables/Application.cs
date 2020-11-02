using Lib;
using System;
using System.Data;
using System.Data.Odbc;
using System.Linq;

namespace LibMESone.Tables
{
    public class Application : BaseID
    {


        [Field(Field.Etype.VarChar, 50, pk: true)]
        public string Key { get; set; }

        [Field(Field.Etype.VarChar, 255)]
        public string Value { get; set; }

/*
        public Application()
        {
            container.TableName = "application";
        }


        

        #region CONSTANTS

        public const string col_name_key = "key";
        public Lib.Database.SExtProp prop_key = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.VarChar,
            size = 50,
            primary_key = true
        };

        public const string col_name_value = "value";
        public Lib.Database.SExtProp prop_value = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.VarChar,
            size = 255
        };



        #endregion

        #region ENUMS

        public enum EKeys
        {
            APPINFO,
            CLOCK

        }

        #endregion



        public Tapplication()
        {
            try
            {
                source.TableName = "application";

                Lib.Database.SExtProp prop = (Lib.Database.SExtProp)source.Columns[Tbase.col_name_id].ExtendedProperties[typeof(Lib.Database.SExtProp)];
                prop.primary_key = false;
                prop.ignore = true;
                source.Columns[Tbase.col_name_id].ExtendedProperties[typeof(Lib.Database.SExtProp)] = prop;
                source.Columns.Add(col_name_key, typeof(string)).ExtendedProperties.Add(prop_key.GetType(), prop_key);
                source.Columns.Add(col_name_value, typeof(string)).ExtendedProperties.Add(prop_value.GetType(), prop_value);

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
        */
    }
}
