using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component.Writers
{
    public interface IWriterCollection
    {
        InterceptWriter Standard { get; }
        InterceptWriter Warning { get; }
        InterceptWriter Error { get; }
        InterceptWriter Debug { get; }
        InterceptWriter Verbose { get; }

        InterceptWriter Accent1 { get; }
        InterceptWriter Accent2 { get; }
        InterceptWriter Accent3 { get; }
        InterceptWriter Accent4 { get; }
        InterceptWriter Accent5 { get; }

        IObjectWriter Object { get; }
        
    }
}
