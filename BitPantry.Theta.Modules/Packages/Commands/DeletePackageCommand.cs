using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPantry.Theta.API;

namespace BitPantry.Theta.Modules.Packages.Commands
{
    [Command("DeletePackage")]
    [Synopsis("Permenantly deletes an package - this cannot be undone")]
    class DeletePackageCommand : InputCommand
    {
        [Parameter(ordinalPosition:1, autoCompleteValuesFunction:"GetPackageNameAutoComplete", useAutoCompleteForValidation:true)]
        [Synopsis("The name of the package to delete")]
        public string Name { get; set; }

        [Switch]
        [Synopsis("Whether or not to force the deletion of the package")]
        public SwitchParameter Force { get; set; }

        public override void Invoke(CommandInvocationContext context)
        {
            if (PackageLogic.Instance.PackagesCollection.Packages.Exists(e => e.Name.Equals(this.Name, StringComparison.OrdinalIgnoreCase)))
            {
                if (PackageLogic.Instance.LoadedPackages.Exists(e => e.Name.Equals(this.Name, StringComparison.OrdinalIgnoreCase))
                    && !this.Force.IsPresent)
                {
                    base.Out.Warning.WriteLine("The package is loaded. Unload the package, or use the 'Force' switch to unload the package before removing.");
                }
                else
                {
                    if (base.Confirm("The package along with all associated modules will be deleted. This cannot be undone.", Component.ConfirmationResult.No))
                    {
                        var pkg = PackageLogic.Instance.PackagesCollection.Packages.FirstOrDefault(e => e.Name.Equals(this.Name, StringComparison.OrdinalIgnoreCase));

                        if (PackageLogic.Instance.LoadedPackages.Exists(e => e.Name.Equals(this.Name, StringComparison.OrdinalIgnoreCase)))
                        {
                            PackageLogic.Instance.Unload(pkg, base.Host.Modules, base.Out);
                            base.Out.Standard.WriteLine("Package unloaded.");
                        }

                        PackageLogic.Instance.Remove(pkg, base.Host.Modules, base.Out);
                        PackageLogic.Instance.Save();

                        base.Out.Standard.WriteLine("Package deleted.");
                    }
                    else
                    {
                        base.Out.Standard.WriteLine("The package has NOT been deleted.");
                    }
                }
            }
            else
            {
                base.Out.Standard.WriteLine("Package could not be found.");
            }
        }

        public void GetPackageNameAutoComplete(AutoCompleteValuesFunctionContext context)
        {
            foreach (var package in PackageLogic.Instance.PackagesCollection.Packages)
                context.Values.Add(package.Name);
        }
    }
}
