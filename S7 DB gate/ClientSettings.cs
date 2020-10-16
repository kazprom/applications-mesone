using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace S7_DB_gate
{
    public class ClientSettings:LibDBgate.BaseSettingsNDE
    {

        public S7.Net.CpuType cpu_type;
        public string ip;
        public int port;
        public short rack;
        public short slot;

        public static void SettingFromDataRow(DataRow row, ref ClientSettings obj)
        {

            LibDBgate.BaseSettingsNDE r_obj = obj;
            LibDBgate.BaseSettingsNDE.SettingFromDataRow(row, ref r_obj);

            obj.cpu_type = (S7.Net.CpuType)Enum.Parse(typeof(S7.Net.CpuType), (string)row[Clients.col_name_cpu_type]);
            obj.ip = (string)row[Clients.col_name_ip];
            obj.port = (int)row[Clients.col_name_port];
            obj.rack = (short)row[Clients.col_name_rack];
            obj.slot = (short)row[Clients.col_name_slot];

        }


    }
}
