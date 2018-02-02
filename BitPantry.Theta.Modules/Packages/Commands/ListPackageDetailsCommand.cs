using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Packages.Commands
{
    [Command("ListPackageDetails")]
    [Synopsis("Lists the details of the given package")]
    class ListPackageDetailsCommand : InputCommand
    {
        [Parameter(ordinalPosition: 1, autoCompleteValuesFunction: "GetPackageNamesAutoComplete", useAutoCompleteForValidation: true)]
        [Synopsis("The name of the package to list details for")]
        public string Package { get; set; }

        public override void Invoke(CommandInvocationContext context)
        {
            var pkg = PackageLogic.Instance.PackagesCollection.Packages
                .FirstOrDefault(e => e.Name.Equals(Package, StringComparison.OrdinalIgnoreCase));

            base.Out.Standard.WriteLine();
            base.Out.Standard.WriteLine(string.Format("     Package:        {0}", pkg.Name));
            base.Out.Standard.WriteLine(string.Format("     Is Loaded:      {0}", PackageLogic.Instance.LoadedPackages.Contains(pkg)));
            base.Out.Object.Table(TableRecords.CreatePackageModuleTableRecordList(pkg.Modules.ToArray()));
        }

        public void GetPackageNamesAutoComplete(AutoCompleteValuesFunctionContext context)
        {
            context.Values.AddRange(PackageLogic.Instance.PackagesCollection.Packages.Select(e => e.Name));
        }
    }
}
