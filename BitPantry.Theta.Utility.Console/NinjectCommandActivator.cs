using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPantry.Theta.API;
using Ninject;

namespace BitPantry.Theta.Utility.Console
{
    public class NinjectCommandActivator : ICommandActivatorContainer
    {
        private readonly IKernel _kernel;

        public NinjectCommandActivator()
        {
            _kernel = new StandardKernel();
        }

        public InputCommand Get(Type inputCommandType)
        {
            return (InputCommand) _kernel.Get(inputCommandType);
        }
    }
}
