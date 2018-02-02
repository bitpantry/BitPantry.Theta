using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Variables.Commands
{
    [Command("OpenVariableContextFile")]
    [Synopsis("Opens the variable context file in the default XML editor")]
    class OpenVariableContextFileCommand : InputCommand
    {
        public override void Invoke(CommandInvocationContext context)
        {
            VariableContextLogic.Instance.DiscardChanges();
            Process.Start(new ProcessStartInfo(VariableContextLogic.FILE_PATH_VARIABLES));
            base.Out.Warning.WriteLine("Any changes made to the variables context file will require a restart of the application to take effect. Also, any updates made to the file from within the console may overwrite your changes.");
        }
    }
}
