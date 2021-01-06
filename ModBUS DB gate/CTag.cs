using System;
using System.Collections.Generic;
using System.Text;

namespace ModBUS_DB_gate
{
    class CTag : LibPlcDBgate.CTag
    {

        #region ENUMS

        public enum EFunctions
        {
            CoilStatus = 1,
            InputStatus = 2,
            HoldingRegister = 3,
            InputRegister = 4
        }

        #endregion

        #region PROPERTIES

        private EFunctions? function;
        public dynamic Function
        {
            get { return function; }
            set
            {
                try
                {
                    if (function == null || function.ToString().ToLower() != Convert.ToString(value).ToLower())
                    {
                        function = Enum.Parse(typeof(EFunctions), Convert.ToString(value), true);
                        Logger.Info($"Function = {function}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
            }
        }

        public ushort? address { get; set; }
        public dynamic Address
        {
            get { return address; }
            set
            {
                try
                {
                    if (address != value)
                    {
                        address = ushort.Parse(Convert.ToString(value));
                        Logger.Info($"Address = {address}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
            }
        }

       
        #endregion

    }
}
