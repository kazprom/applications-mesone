using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XML_DB_gate
{
    class Logger
    {
        private static Logger instance;

        private string path_to_dir = $@"{Program.PathExeFolder}LOG";

        private Logger() { }

        public static Logger GetInstance()
        {
            if (instance == null)
                instance = new Logger();


            return instance;
        }

        #region WriteLogs

        public void WriteMessage(long id, string message, Exception ex = null, bool write_to_db = true)
        {
            try
            {
                string msg = "";

                if (message != "")
                    msg += message;

                if (ex != null)
                    msg += (msg != "" ? "\n" : "") + ex.Message;

                if (msg != "")
                    Console.WriteLine($"[{DateTime.Now}] <{id}> {msg}");

                if (ex != null)
                    msg += (msg != "" ? "\n" : "") + ex.StackTrace;


                if (!Directory.Exists(path_to_dir))
                {
                    DirectoryInfo di = Directory.CreateDirectory(path_to_dir);
                }

                using (StreamWriter myStream = new StreamWriter(path_to_dir + Path.DirectorySeparatorChar + DateTime.Now.ToString("yyyy_MM_dd") + ".log", true))
                {
                    myStream.WriteLine($"[{DateTime.Now}]");
                    myStream.WriteLine($"Programm ID={id}");
                    myStream.WriteLine(msg);
                }
            }
            catch (Exception) { }
        }

        #endregion

        #region DeleteFiles

        public void DeleteOldLogFiles(int depth)
        {
            try
            {
                if (!File.Exists(path_to_dir))
                {
                    var dir = new DirectoryInfo(path_to_dir);

                    foreach (FileInfo file in dir.GetFiles("*.log"))
                    {
                        string[] ts_arr = file.Name.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0].Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

                        int ts_year = 0, ts_month = 0, ts_day = 0;

                        if (int.TryParse(ts_arr[0], out ts_year) && int.TryParse(ts_arr[1], out ts_month) && int.TryParse(ts_arr[2], out ts_day))
                        {
                            DateTime ts_file = new DateTime(ts_year, ts_month, ts_day);

                            if (DateTime.Now.Subtract(ts_file).TotalDays >= depth)
                            {
                                File.Delete($@"{file.FullName}");
                                WriteMessage(0, $"Deleted log file {ts_file.ToString("yyyy_MM_dd")}.log");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteMessage(0, "Logger Error delete old log files", ex);
            }
        }

        #endregion
    }
}
