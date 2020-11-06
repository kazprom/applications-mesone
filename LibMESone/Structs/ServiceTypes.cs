using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMESone.Structs
{
    public class ServiceTypes : BaseID
    {
        [Field(Field.Etype.VarChar, 255, UQ = true)]
        public string Name { get; set; }

        [Field(Field.Etype.VarChar, 255)]
        public string Guid { get; set; }
        /*

        public ServiceTypes()
        {
            container.TableName = "service_types";
        }


        #region CONSTANTS

        public const string col_name_name = "name";
        public Lib.Database.SExtProp prop_name = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.VarChar,
            size = 255,
            primary_key = true
        };

        public const string col_name_guid = "GUID";
        public Lib.Database.SExtProp prop_guid = new Lib.Database.SExtProp()
        {
            data_type = System.Data.Odbc.OdbcType.VarChar,
            size = 40
        };

        #endregion


        #region VARIABLES


        #endregion


        public Tservice_types()
        {
            try
            {

                source.TableName = "service_types";

                source.Columns.Add(col_name_name, typeof(string)).ExtendedProperties.Add(prop_name.GetType(), prop_name);
                source.Columns.Add(col_name_guid, typeof(string)).ExtendedProperties.Add(prop_guid.GetType(), prop_guid);

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
        */
    }
}
