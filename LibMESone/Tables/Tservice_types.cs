using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMESone.Tables
{
    public class Tservice_types : Tbase
    {

        #region CONSTANTS

        public const string col_name_name = "name";
        public readonly static Type data_type_name = typeof(string);

        public const string col_name_guid = "GUID";
        public readonly static Type data_type_guid = typeof(string);

        #endregion


        #region VARIABLES


        #endregion


        public Tservice_types()
        {
            try
            {

                source.TableName = "service_types";

                source.Columns.Add(col_name_name, data_type_name).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
                {
                    data_type = System.Data.Odbc.OdbcType.VarChar,
                    size = 255,
                    primary_key = true
                });
                source.Columns.Add(col_name_guid, data_type_guid).ExtendedProperties.Add(typeof(Lib.Database.SExtProp), new Lib.Database.SExtProp()
                {
                    data_type = System.Data.Odbc.OdbcType.VarChar,
                    size = 40,
                });

            }
            catch (Exception ex)
            {

                throw new Exception("Error constructor", ex);

            }




        }


        public long GetIDbyGUID(string guid)
        {
            try
            {
                return (long)source.Select($"{col_name_guid} = '{guid}'").FirstOrDefault()[col_name_id];
            }
            catch (Exception ex)
            {
                throw new Exception("Error get id by GUID", ex);
            }
        }

    }
}
