using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibMESone.Loggers
{
    public class TextLogger
    {
        #region CONSTANTS

        public const string format = "log";
        public const char separator = '_';

        #endregion

        #region PROPERTIES

        private string path;
        public string Path { get { return path; } set { path = value; Lib.Message.Make($"Path to LOG = {path}"); } }

        #endregion

        #region CONSTRUCTOR

        public TextLogger() : this($@"{Lib.Global.PathExeFolder}LOG") { }

        public TextLogger(string path)
        {
            Path = path;
            Lib.Message.FullMsgMaked += Write;
        }

        #endregion



        private void Write(string str)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    DirectoryInfo di = Directory.CreateDirectory(path);
                }

                using (StreamWriter myStream = new StreamWriter(path + System.IO.Path.DirectorySeparatorChar + GetFileName(DateTime.Now), true))
                {
                    myStream.WriteLine($"[{DateTime.Now}]");
                    myStream.WriteLine(str);
                    myStream.WriteLine();
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error write text log {ex.Message}");
            }


        }

        public static string GetFileName(DateTime timestamp)
        {
            return timestamp.ToString("yyyy" + separator + "MM" + separator + "dd") + "." + format;
        }


    }
}
