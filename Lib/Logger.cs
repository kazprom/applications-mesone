using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lib
{
    public class Logger
    {


        private static string folder = $@"{Global.PathExeFolder}LOG";
        public static string Folder { get { return folder; } }


        #region CONSTRUCTOR

        public Logger()
        {

        }

        #endregion


        #region PUBLICS

        public static void WriteMessage(string message, Exception ex = null)
        {
            try
            {
                string msg = "";

                if (message != "")
                    msg += message;

                Exception ex_ref = ex;
                if (ex_ref != null)
                {
                    msg += (msg != "" ? "\n" : "");
                    while (ex_ref != null)
                    {
                        msg += $"[{ex_ref.Message}]";
                        if (ex_ref.InnerException != null)
                            msg += " -> ";
                        ex_ref = ex_ref.InnerException;
                    }
                }

                if (msg != "")
                    Console.WriteLine($"[{DateTime.Now}] {msg}");

                if (ex != null)
                    msg += (msg != "" ? "\n" : "") + ex.StackTrace;



                if (!Directory.Exists(folder))
                {
                    DirectoryInfo di = Directory.CreateDirectory(folder);
                }

                using (StreamWriter myStream = new StreamWriter(folder + Path.DirectorySeparatorChar + DateTime.Now.ToString("yyyy_MM_dd") + ".log", true))
                {
                    myStream.WriteLine($"[{DateTime.Now}]");
                    myStream.WriteLine(msg);
                }

            }
            catch (Exception)
            {
                Console.WriteLine($"Logger error write message [{ex.Message}]");
            }
        }

        #endregion


    }
}
