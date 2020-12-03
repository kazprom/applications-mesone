using System;
using System.Collections.Generic;
using System.Text;

namespace LibDBgate
{
    public class CSrvCUSTOM : LibMESone.CSrvCUSTOM
    {

        #region VARIABLES

        public Dictionary<ulong, CSrvSUB> SubServices = new Dictionary<ulong, CSrvSUB>();

        #endregion

        
        #region DESTRUCTOR
        
        public override void Dispose(bool disposing)
        {

            foreach (CSrvSUB sub_service in SubServices.Values)
            {
                sub_service.Dispose();
            }

            SubServices.Clear();

            base.Dispose(disposing);    
        }

        #endregion

    }
}
