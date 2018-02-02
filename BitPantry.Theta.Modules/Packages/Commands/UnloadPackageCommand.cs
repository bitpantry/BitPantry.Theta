using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Packages.Commands
{
    [Command("UnloadPackage")]
    [Synopsis("Unloads a package")]
    class UnloadPackageCommand : InputCommand
    {
        [Parameter(ordinalPosition:1, autoCompleteValuesFunction:"GetLoadedPackageNames", useAutoCompleteForValidation:true)]
        public string Name { get; set; }

        public override void Invoke(CommandInvocationContext context)
        {
            PackageLogic.Instance.Unload(PackageLogic.Instance.LoadedPackages
                .FirstOrDefault(e => e.Name.Equals(this.Name, StringComparison.OrdinalIgnoreCase)), base.Host.Modules, base.Out);
            base.Out.Standard.WriteLine("Package unloaded.");
        }

        public void GetLoadedPackageNames(AutoCompleteValuesFunctionContext context)
        {
            context.Values.AddRange(PackageLogic.Instance.LoadedPackages.Select(e => e.Name).ToList());
        }
    }
}
