using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lib
{
    public class Console
    {
        #region CONSTRUCTOR

        public Console()
        {
            Message.ShortMsgMaked += Show;
        }

        #endregion


        #region PUBLICS

        private static void Show(string str)
        {
            System.Console.WriteLine($"[{DateTime.Now}] {str}");
        }

        #endregion


    }
}
