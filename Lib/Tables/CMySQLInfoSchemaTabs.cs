using System;
using System.Collections.Generic;
using System.Text;

namespace Lib.Tables
{
    class CMySQLInfoSchemaTabs
    {

        static public string TableName = "INFORMATION_SCHEMA.TABLES";

        public string Table_schema { get; set; }

        public string Table_name { get; set; }

    }
}
