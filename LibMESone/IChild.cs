using System;
using System.Collections.Generic;
using System.Text;

namespace LibMESone
{
    public interface IChild: IDisposable
    {
        ulong ID { get; set; }

        IParent Parent { get; set; }

        void LoadSetting(ISetting setting);

    }
}
