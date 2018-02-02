using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.API
{
    public interface ISwitchDefinition
    {
        string Name { get; }
        IEnumerable<string> Aliases { get; }
    }
}
