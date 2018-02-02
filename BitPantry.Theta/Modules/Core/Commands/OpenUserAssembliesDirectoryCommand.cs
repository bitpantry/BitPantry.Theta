using BitPantry.Theta.API;
using BitPantry.Theta.Component;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Core.Commands
{
    [Command("OpenUserAssembliesDirectory")]
    [Synopsis("Opens File Explorer to the user's assembly directory")]
    class OpenUserAssembliesDirectoryCommand : InputCommand
    {
        public override void Invoke(CommandInvocationContext context)
        {
            Process.Start(new ProcessStartInfo("explorer.exe", new UserAssemblyRepository().UserAssemblyDirectoryPath));
        }
    }
}
