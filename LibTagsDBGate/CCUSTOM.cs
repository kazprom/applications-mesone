
namespace LibPlcDBgate
{
    public class CCUSTOM : LibDBgate.CCUSTOM
    {

        public CHistorian Historian { get; set; } = new CHistorian();

        public CRTviewer RTviewer { get; set; } = new CRTviewer();

        public CCUSTOM()
        {

            LoggerMaked += CCUSTOM_LoggerMaked;

            Historian.DB = DB;
            Historian.Parent = this;

            RTviewer.DB = DB;
            RTviewer.Parent = this;

        }

        private void CCUSTOM_LoggerMaked(NLog.Logger logger)
        {
            Historian.Logger = Logger;
            RTviewer.Logger = Logger;
        }

    }
}
