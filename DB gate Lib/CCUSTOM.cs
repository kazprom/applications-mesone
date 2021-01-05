using System;
using System.Collections.Generic;
using System.Text;

namespace LibDBgate
{
    public class CCUSTOM: LibMESone.CCUSTOM
    {

        public CDiagnoster Diagnoster { get; set; } = new CDiagnoster();


        public CCUSTOM()
        {

            LoggerMaked += CCUSTOM_LoggerMaked;

            Diagnoster.DB = DB;
            Diagnoster.Parent = this;

        }

        private void CCUSTOM_LoggerMaked(NLog.Logger logger)
        {
            Diagnoster.Logger = Logger;
        }


    }
}
