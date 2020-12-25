using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace OPC_DB_gate_server
{
    class HistoryCleaner
    {

        #region VARIABLE


        private Lib.CDatabase database;
        private Lib.Parameter<int> depth_hour;

        private Thread thread;
        private bool execution = true;


        #endregion


        public HistoryCleaner(Lib.CDatabase database, Lib.Parameter<int> depth_hour)
        {
            this.database = database;
            this.depth_hour = depth_hour;

            thread = new Thread(new ThreadStart(Handler)) { IsBackground = true, Name = "History cleaner" };
            thread.Start();

        }



        private void Handler()
        {
            while (execution)
            {
                try
                {

                    if (DateTime.Now.Minute == 0)
                    {

                        foreach (DateTime timestamp in GetTimestamps())
                        {
                            if (DateTime.Now.Subtract(timestamp).TotalHours >= depth_hour.Value)
                                DeleteTable(timestamp);
                        }

                        Thread.Sleep(60000);
                    }

                }
                catch (Exception ex)
                {
                    Lib.Message.Make("Error clean log table", ex);
                }

                Thread.Sleep(30000);
            }

        }


        private DateTime[] GetTimestamps()
        {
            try
            {
                List<DateTime> result = new List<DateTime>();

                foreach (string table in database.GetListTables($"TABLE_NAME Like '{History.table_prefix}{History.separator} %' "))
                {
                    string[] part_timestamp = table.Split(History.separator);
                    if (part_timestamp.Length == 5)
                    {
                        int year, month, day, hour;

                        if (int.TryParse(part_timestamp[1], out year) &&
                            int.TryParse(part_timestamp[2], out month) &&
                            int.TryParse(part_timestamp[3], out day) &&
                            int.TryParse(part_timestamp[4], out hour))
                            result.Add(new DateTime(year, month, day, hour, 0, 0));

                    }
                }
                return result.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception("Error get timestamps", ex);
            }

        }

        private void DeleteTable(DateTime timestamp)
        {
            try
            {
                string table = History.GetTableName(timestamp);
                database.DeleteTable(table);
                Lib.Message.Make($"Deleted table {table}");
            }
            catch (Exception ex)
            {
                throw new Exception("Error delete table", ex);
            }
        }

    }
}
