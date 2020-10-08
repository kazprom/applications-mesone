using System;
using System.Collections.Generic;
using System.Text;

namespace Lib
{
    public class Message
    {

        #region EVENTS

        public delegate void ExNotify(Exception ex);  // delegate
        public static event ExNotify ExMaked; // event

        public delegate void TxtNotify(string str);  // delegate
        public static event TxtNotify ShortMsgMaked; // event
        public static event TxtNotify FullMsgMaked; // event

        #endregion

        public static void Make(string message, Exception ex = null)
        {

            try
            {
                if (ex != null)
                    ExMaked?.Invoke(ex);

                string msg = String.Empty;

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


                ShortMsgMaked?.Invoke($"{message}" + (ex != null ? $"\n {msg}" : ""));
                FullMsgMaked?.Invoke($"{message}" + (ex != null ? $"\n {msg} \n {ex.StackTrace}" : ""));

            }
            catch (Exception e)
            {
                System.Console.WriteLine($"Error make message [{e.Message}]");
            }
        }
    }
}
