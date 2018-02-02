using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Packages.Commands
{
    [Command("RemovePackagesModule")]
    [Synopsis("Removes the specified package module")]
    class RemovePackageModuleCommand : InputCommand
    {
        [Parameter(ordinalPosition: 1, autoCompleteValuesFunction: "GetPackageNamesAutoComplete", useAutoCompleteForValidation: true)]
        [Synopsis("The name of the package to remove the module from")]
        public string Package { get; set; }

        [Parameter(ordinalPosition: 2, autoCompleteValuesFunction: "GetModuleNameAutoComplete", useAutoCompleteForValidation:true)]
        [Synopsis("The name of the module to remove")]
        public string Module { get; set; }

        public override void Invoke(CommandInvocationContext context)
        {
            if (base.Confirm("The module will be removed."))
            {
                var pkg = PackageLogic.Instance.PackagesCollection.Packages
                    .FirstOrDefault(e => e.Name.Equals(Package, StringComparison.OrdinalIgnoreCase));

                pkg.Modules.Remove(pkg.Modules.FirstOrDefault(m => m.Name.Equals(Module, StringComparison.OrdinalIgnoreCase)));

                PackageLogic.Instance.Save();

                base.Out.Object.Table(TableRecords.CreatePackageRecordList(pkg));

                if (PackageLogic.Instance.LoadedPackages.Contains(pkg))
                    base.Out.Warning.WriteLine("This package is currently loaded and must be reloaded before the changes will take effect");
            }
        }

        public void GetPackageNamesAutoComplete(AutoCompleteValuesFunctionContext context)
        {
            context.Values.AddRange(PackageLogic.Instance.PackagesCollection.Packages.Select(e => e.Name));
        }

        public void GetModuleNameAutoComplete(AutoCompleteValuesFunctionContext context)
        {
            if (string.IsNullOrWhiteSpace(Package))
                return;

            context.Values.AddRange(
                PackageLogic.Instance.PackagesCollection.Packages
                .FirstOrDefault(e => e.Name.Equals(Package, StringComparison.OrdinalIgnoreCase))
                .Modules.Select(m => m.Name));
        }
    }
}
