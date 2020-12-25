using NLog;

namespace LibMESone
{
    public class CSrv : CParent
    {

        private ulong id;
        public ulong Id { get { return id; } set { if (id != value) { id = value; SetLogger(); } } }

        private string name;
        public string Name { get { return name; } set { if (name != value) { name = value; SetLogger(); } } }


        private void SetLogger()
        {
            if (Parent != null && Parent.Logger != null)
            {
                Logger = LogManager.GetLogger($"{Parent.Logger.Name}.{Id}");
            }
            else
            {
                Logger = LogManager.GetLogger($"{Id}");
            }

        }


    }
}
