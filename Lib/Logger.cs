using System;
using System.Collections.Generic;
using System.Text;

namespace Lib
{
    public class Logger
    {

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

                /*
                string path_to_folder = $@"{Lib.Global.PathExeFolder}LOG";

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

                */
            }
            catch (Exception) { }
        }

        #endregion


    }
}
