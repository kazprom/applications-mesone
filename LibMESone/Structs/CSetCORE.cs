
namespace LibMESone.Structs
{
    public class CSetCORE : CSetting
    {

        public string DB_Driver { get; set; }
        public string DB_Host { get; set; }
        public ushort DB_Port { get; set; }
        public string DB_Charset { get; set; }
        public string DB_BaseName { get; set; }
        public string DB_User { get; set; }
        public string DB_Password { get; set; }


        public uint LOG_DepthDay { get; set; }

    }
}
