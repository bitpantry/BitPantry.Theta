using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPantry.Theta.Host.WindowsForms.InputEventsFilter
{
    class InputEventsFilterHandlerArgs
    {
        public KeyInputFilterResult Result { get; set; }
        public bool IsHandled { get; set; }

        public InputEventsFilterHandlerArgs(KeyInputFilterResult result)
        {
            Result = result;
            IsHandled = false;
        }
    }
}
