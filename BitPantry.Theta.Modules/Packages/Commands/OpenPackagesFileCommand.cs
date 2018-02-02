using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Packages.Commands
{
    [Command("OpenPackagesFile")]
    [Synopsis("Opens the packages file in the default XML editor")]
    class OpenPackagesFileCommand : InputCommand
    {
        public override void Invoke(CommandInvocationContext context)
        {
            PackageLogic.Instance.DiscardChanges();
            Process.Start(new ProcessStartInfo(PackageLogic.FILE_PATH_PACKAGES));
            base.Out.Warning.WriteLine("Any changes made to the packages file will require a restart of the application to take effect. Also, any updates made to the packages from within the console may overwrite your changes.");
        }
    }
}
