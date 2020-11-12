using System;
using System.Collections.Generic;
using System.Text;

namespace S7_DB_gate
{
    public class Tag : LibDBgate.Tag
    {

        #region PROPERTIES

        public S7.Net.DataType DataType { get; set; }
        public int DB { get; set; }
        public int StartByteAdr { get; set; }
        public S7.Net.VarType VarType { get; set; }
        public byte BitAdr { get; set; }
        public EDataType TagType { get; set; }
        public bool RT_enabled { get; set; }
        public bool History_enabled { get; set; }

        #endregion

        #region CONSTRUCTOR
        public Tag(Group parent, ulong id) : base(parent, id)
        {
        }

        #endregion

        #region PUBLICS

        public override void LoadSettings(dynamic tag)
        {
            if (tag != null)
            {

                bool changed = false;
                string title = "";

                try
                {
                    Structs.Tag _tag = tag;

                    title += $" Name [{_tag.Name}]";
                    if (Name != _tag.Name)
                    {
                        Name = _tag.Name;
                        changed = true;
                    }

                    title += $" PLC data type [{_tag.PLC_data_type}]";
                    S7.Net.DataType res_dt = (S7.Net.DataType)Enum.Parse(typeof(S7.Net.DataType), _tag.PLC_data_type, true);
                    if (DataType != res_dt)
                    {
                        DataType = res_dt;
                        changed = true;
                    }


                    title += $" Data block no. [{_tag.Data_block_no}]";
                    if (DB != _tag.Data_block_no)
                    {
                        DB = _tag.Data_block_no;
                        changed = true;
                    }

                    title += $" Data block offset [{_tag.Data_block_offset}]";
                    if (StartByteAdr != _tag.Data_block_offset)
                    {
                        StartByteAdr = _tag.Data_block_offset;
                        changed = true;
                    }

                    title += $" Request type [{_tag.Request_type}]";
                    S7.Net.VarType res_vt = (S7.Net.VarType)Enum.Parse(typeof(S7.Net.VarType), _tag.Request_type, true);
                    if (VarType != res_vt)
                    {
                        VarType = res_vt;
                        changed = true;
                    }

                    title += $" Bit offset [{_tag.Bit_offset}]";
                    if (BitAdr != _tag.Bit_offset)
                    {
                        BitAdr = _tag.Bit_offset;
                        changed = true;
                    }

                    title += $" Data type [{_tag.Data_type}]";
                    LibDBgate.Tag.EDataType res_sdt = (LibDBgate.Tag.EDataType)Enum.Parse(typeof(LibDBgate.Tag.EDataType), _tag.Data_type, true);
                    if (TagType != res_sdt)
                    {
                        TagType = res_sdt;
                        changed = true;
                    }

                    if (RT_enabled != _tag.RT_values_enabled)
                    {
                        RT_enabled = _tag.RT_values_enabled;
                        changed = true;
                    }

                    if (History_enabled != _tag.History_enabled)
                    {
                        History_enabled = _tag.History_enabled;
                        changed = true;
                    }

                    if (changed)
                        logger.Info($"{Title}. Changed");

                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"{title}. Load settings");
                }

            }

        }

        #endregion

    }
}
