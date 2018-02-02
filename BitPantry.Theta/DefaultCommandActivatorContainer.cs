using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPantry.Theta.API;

namespace BitPantry.Theta
{
    class DefaultCommandActivatorContainer : ICommandActivatorContainer
    {
        public InputCommand Get(Type inputCommandType)
        {
            return (InputCommand) Activator.CreateInstance(inputCommandType);
        }
    }
}
