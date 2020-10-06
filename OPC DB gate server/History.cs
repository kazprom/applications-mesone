using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OPC_DB_gate_server
{
    class History
    {

        #region CONSTANTS

        public const string table_prefix = "t_";

        public const string col_name_tags_id = "tags_id";
        public const string col_name_timestamp = "timestamp";
        public const string col_name_value = "value";
        public const string col_name_quality = "quality";

        #endregion



        #region PROPERTIES

        private DataSet source = new DataSet("history");
        public DataSet Source { get { return source; } }

        #endregion



        public void Put(OPC_DB_gate_Lib.TagData tag)
        {
            try
            {
                //Console.WriteLine("history");
            }
            catch (Exception ex)
            {
                throw new Exception("Error put", ex);
            }
        }

    }
}
