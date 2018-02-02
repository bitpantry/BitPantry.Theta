using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Processing
{
    class CommandInvokerResponse
    {
        public InputCommand Command { get; set; }
        public Exception InvocationError { get; set; }
    }
}
