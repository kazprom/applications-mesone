using System;
using System.Collections.Generic;
using System.Text;

namespace LibMESone
{
    public interface IParent: IDisposable
    {

        NLog.Logger Logger { get; set; }

        Dictionary<ulong, IChild> Children { get; set; }

    }
}
