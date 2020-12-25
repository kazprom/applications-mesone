using System;
using System.Collections.Generic;
using System.Text;

namespace LibPlcDBgate
{
    public class CSrv : LibMESone.CCUSTOM
    {

        public CHistorian Historian { get; set; }


        public CSrv()
        {
            try
            {
                Historian = new CHistorian();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }


    }
}
