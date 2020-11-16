using System;
using System.Collections.Generic;
using System.Text;

namespace Lib.Structs
{
    class MySQLInfoSchemaCols
    {

        public string Table_schema { get; set; }

        public string Table_name { get; set; }

        public string Column_name { get; set; }

        public string Column_type { get; set; }

        public string Column_key { get; set; }

        public string Extra { get; set; }

        public string Is_nullable { get; set; }

    }
}
