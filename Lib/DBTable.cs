using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace Lib
{
    public class DBTable
    {

        public const string col_name_id = "id";


        private DataTable table;
        public DataTable Table { get { return table; } }

        public DBTable(string name)
        {
            try
            {
                table = new DataTable(name);
                table.PrimaryKey = new DataColumn[] { table.Columns.Add(col_name_id, typeof(int)) };
            }
            catch (Exception ex)
            {
                throw new Exception("Error make table", ex);
            }


        }

        public void AddColumn(string name, Type type)
        {
            table.Columns.Add(name, type);
        }

    }
}
