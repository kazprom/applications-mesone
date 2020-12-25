using System;
using System.Collections.Generic;
using System.Text;

namespace LibMESone
{
    public class CSrvDB: CSrvCyc
    {

        public Lib.CDatabase Database { get; set; }


        public CSrvDB()
        {
            Database = new Lib.CDatabase();
            Database.Logger = Logger;
        }

    }
}
