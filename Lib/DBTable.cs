using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace Lib
{
    public class DBTable
    {

        public enum EColumns
        {
            id
        }


        private DataTable table;
        public DataTable Table { get { return table; } }

        public DBTable(string name)
        {
            try
            {
                table = new DataTable(name);
                table.PrimaryKey = new DataColumn[] { table.Columns.Add(EColumns.id.ToString(), typeof(int)) };
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
