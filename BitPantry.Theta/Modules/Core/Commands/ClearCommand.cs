using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitPantry.Theta.Modules.Core.Commands
{
    [Command("Clear")]
    [Alias("cls")]
    [Synopsis("Clears the console")]
    class ClearCommand : InputCommand
    {
        public override void Invoke(CommandInvocationContext context)
        {
            base.Host.Clear();
        }
        
    }
}
