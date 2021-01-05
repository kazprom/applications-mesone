using LibMESone;
using System;
using System.Collections.Generic;
using System.Text;

namespace S7_DB_gate
{
    public class CTag : LibPlcDBgate.CTag
    {

        #region PROPERTIES

        private S7.Net.DataType? data_type;
        public dynamic S7_Data_Type
        {
            get { return data_type; }
            set
            {
                try
                {
                    if (data_type == null || data_type.ToString().ToLower() != Convert.ToString(value).ToLower())
                    {
                        data_type = Enum.Parse(typeof(S7.Net.DataType), Convert.ToString(value), true);
                        Logger.Info($"S7 data type = {data_type}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
            }
        }

        public int? db { get; set; }
        public dynamic DB
        {
            get { return db; }
            set
            {
                try
                {
                    if (db != value)
                    {
                        db = int.Parse(Convert.ToString(value));
                        Logger.Info($"Data block no = {db}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
            }
        }

        private int? start_byte_adr;
        public dynamic StartByteAdr
        {
            get { return start_byte_adr; }
            set
            {
                try
                {
                    if (start_byte_adr != value)
                    {
                        start_byte_adr = int.Parse(Convert.ToString(value));
                        Logger.Info($"Start byte address = {start_byte_adr}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
            }
        }

        private S7.Net.VarType? s7_var_type;
        public dynamic S7_Var_Type
        {
            get { return s7_var_type; }
            set
            {
                try
                {
                    if (s7_var_type == null || s7_var_type.ToString().ToLower() != Convert.ToString(value).ToLower())
                    {
                        s7_var_type = Enum.Parse(typeof(S7.Net.VarType), Convert.ToString(value), true);
                        Logger.Info($"S7 var type = {s7_var_type}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
            }
        }

        public byte? bit_adr;
        public dynamic BitAdr
        {
            get { return bit_adr; }
            set
            {
                try
                {
                    if (bit_adr != value)
                    {
                        bit_adr = byte.Parse(Convert.ToString(value));
                        Logger.Info($"Bit address = {bit_adr}");
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
