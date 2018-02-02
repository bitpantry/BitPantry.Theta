using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Packages.Commands
{
    [Command("LoadPackage")]
    [Synopsis("Loads the specified package")]
    class LoadPackageCommand : InputCommand
    {
        [Parameter(ordinalPosition: 1, autoCompleteValuesFunction: "GetPackageNamesAutoComplete", useAutoCompleteForValidation: true)]
        [Synopsis("The name of the package to list details for")]
        public string Package { get; set; }

        public override void Invoke(CommandInvocationContext context)
        {
            PackageLogic.Instance.Load(PackageLogic.Instance.PackagesCollection.Packages
                .FirstOrDefault(e => e.Name.Equals(Package, StringComparison.OrdinalIgnoreCase)), base.Host.Modules, base.Out);
        }

        public void GetPackageNamesAutoComplete(AutoCompleteValuesFunctionContext context)
        {
            context.Values.AddRange(PackageLogic.Instance.PackagesCollection.Packages.Select(e => e.Name));
        }

    }
}
