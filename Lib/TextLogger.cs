using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lib
{
    public class TextLogger
    {
        #region CONSTANTS

        public const string format = "log";
        public const char separator = '_';

        #endregion


        #region PROPERTIES

        private string path;
        public string Path { get { return path; } set { path = value; } }

        #endregion

        public TextLogger(string path)
        {
            this.path = path;

            Message.FullMsgMaked += Write;

        }

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
                    myStream.WriteLine($"{DateTime.Now}");
                    myStream.WriteLine(str);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error write", ex);
            }


        }

        public static string GetFileName(DateTime timestamp)
        {
            return timestamp.ToString("yyyy" + separator + "MM" + separator + "dd") + "." + format;
        }


    }
}
