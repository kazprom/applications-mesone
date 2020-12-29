
namespace LibPlcDBgate
{
    public class CCUSTOM : LibMESone.CCUSTOM
    {

        public CHistorian Historian { get; set; } = new CHistorian();

        public CRTviewer RTviewer { get; set; } = new CRTviewer();

        public CCUSTOM()
        {
            Historian.DB = DB;
            RTviewer.DB = DB;
        }

    }
}
