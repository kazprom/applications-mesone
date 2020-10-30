using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LibMESone.Loggers
{
    public class DBLogCleaner
    {
        public const int default_depth_day = 2;


        #region VARIABLES

        private DBLogger logger;
        private Lib.Parameter<int> depth_day;

        private Thread thread;
        private bool execution = true;

        #endregion

        public DBLogCleaner(DBLogger logger, Lib.Parameter<int> depth_day)
        {

            this.logger = logger;
            this.depth_day = depth_day;

            thread = new Thread(new ThreadStart(Handler)) { IsBackground = true, Name = "DB log cleaner" };
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
                            if (DateTime.Now.Subtract(timestamp).TotalDays >= depth_day.Value)
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
                /*
                foreach (string table in logger.Database.GetListTables($"TABLE_NAME Like '{DBLogger.table_prefix}{DBLogger.separator} %' "))
                {
                    string[] part_timestamp = table.Split(DBLogger.separator);
                    if (part_timestamp.Length == 4)
                    {
                        int year, month, day;

                        if (int.TryParse(part_timestamp[1], out year) &&
                            int.TryParse(part_timestamp[2], out month) &&
                            int.TryParse(part_timestamp[3], out day))
                            result.Add(new DateTime(year, month, day));

                    }
                }
                */
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
                string table = DBLogger.GetTableName(timestamp);
                //logger.Database.DeleteTable(table);
                Lib.Message.Make($"Deleted table {table}");
            }
            catch (Exception ex)
            {
                throw new Exception("Error delete table", ex);
            }
        }


    }
}
