using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace LibMESone.Loggers
{
    public class TextLogCleaner
    {


        public const int default_depth_day = 2;


        #region VARIABLES

        private TextLogger logger;
        private Lib.Parameter<int> depth_day;

        private Thread thread;
        private bool execution = true;

        #endregion

        public TextLogCleaner(TextLogger logger, Lib.Parameter<int> depth_day)
        {

            this.logger = logger;
            this.depth_day = depth_day;

            thread = new Thread(new ThreadStart(Handler)) { IsBackground = true, Name = "Text log cleaner" };
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
                                DeleteFile(timestamp);
                        }

                        Thread.Sleep(60000);
                    }

                }
                catch (Exception ex)
                {
                    //Lib.Message.Make("Error clean log file", ex);
                }

                Thread.Sleep(30000);
            }
        }

        private DateTime[] GetTimestamps()
        {
            try
            {
                List<DateTime> result = new List<DateTime>();

                foreach (string file in Directory.GetFiles(logger.Path, "*." + TextLogger.format))
                {
                    string[] part_timestamp = Path.GetFileName(file).Split('.')[0].Split(TextLogger.separator);
                    if (part_timestamp.Length == 3)
                    {
                        int year, month, day;

                        if (int.TryParse(part_timestamp[0], out year) &&
                           int.TryParse(part_timestamp[1], out month) &&
                           int.TryParse(part_timestamp[2], out day))
                            result.Add(new DateTime(year, month, day));

                    }
                }
                return result.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception("Error get timestamps", ex);
            }

        }

        private void DeleteFile(DateTime timestamp)
        {
            try
            {
                string file = logger.Path + System.IO.Path.DirectorySeparatorChar + TextLogger.GetFileName(timestamp);
                File.Delete(file);
                //Lib.Message.Make($"Deleted file {file}");
            }
            catch (Exception ex)
            {
                throw new Exception("Error delete file", ex);
            }
        }

    }
}
