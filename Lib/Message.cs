﻿using System;
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

                string msg_ex = String.Empty;
                string msg_stack_trace = String.Empty;

                Exception ex_ref = ex;
                if (ex_ref != null)
                {
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex_ref, true);

                    while (ex_ref != null)
                    {
                        msg_ex += $"[{trace.GetFrame(0).GetMethod().ReflectedType.FullName} - {ex_ref.Message}]";
                        msg_stack_trace += $"{ex_ref.StackTrace}";
                        if (ex_ref.InnerException != null)
                        {
                            msg_ex += " -> ";
                            msg_stack_trace += "\n";
                        }
                        ex_ref = ex_ref.InnerException;
                    }
                }

                ShortMsgMaked?.Invoke($"{message}" + (ex != null ? $"\n{msg_ex}" : ""));
                FullMsgMaked?.Invoke($"{message}" + (ex != null ? $"\n{msg_ex}\n{msg_stack_trace}" : ""));

            }
            catch (Exception e)
            {
                System.Console.WriteLine($"Error make message [{e.Message}]");
            }
        }
    }
}
