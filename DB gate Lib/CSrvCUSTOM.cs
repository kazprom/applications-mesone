using System;
using System.Collections.Generic;
using System.Text;

namespace LibDBgate
{
    public class CSrvCustom : LibMESone.CCUSTOM
    {

        #region VARIABLES

        public Dictionary<ulong, CSrvSub> SubServices = new Dictionary<ulong, CSrvSub>();

        #endregion

        
        #region DESTRUCTOR
        
        public override void Dispose(bool disposing)
        {

            foreach (CSrvSub sub_service in SubServices.Values)
            {
                sub_service.Dispose();
            }

            SubServices.Clear();

            base.Dispose(disposing);    
        }

        #endregion

    }
}
