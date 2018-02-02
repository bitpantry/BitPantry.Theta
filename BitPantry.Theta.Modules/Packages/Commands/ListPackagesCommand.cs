using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPantry.Theta.Extensions;

namespace BitPantry.Theta.Modules.Packages.Commands
{
    [Command("ListPackages")]
    [Alias("lp")]
    [Synopsis("Lists the currently available packages as well as which packages are loaded")]
    class ListPackagesCommand : InputCommand
    {
        [Parameter(ordinalPosition:1, isRequired:false)]
        [Synopsis("Filters the list of available packages")]
        public string Filter { get; set; }

        [Switch]
        public SwitchParameter LoadedOnly { get; set; }

        public override void Invoke(CommandInvocationContext context)
        {            
            if (PackageLogic.Instance.PackagesCollection.Packages.Count > 0)
            {
                List<Packages.Package> packagesToList = new List<Packages.Package>();

                foreach (var pkg in PackageLogic.Instance.PackagesCollection.Packages)
                {
                    bool isLoaded = PackageLogic.Instance.LoadedPackages.Contains(pkg);
                    bool add = true;
                    if (LoadedOnly.IsPresent && !isLoaded)
                        add = false;

                    if (add)
                        packagesToList.Add(pkg);

                }

                if (!string.IsNullOrWhiteSpace(Filter))
                    packagesToList = packagesToList.Where(e => e.Name.IndexOf(Filter, StringComparison.OrdinalIgnoreCase) > -1).ToList();

                base.Out.Object.Write(TableRecords.CreatePackageRecordList(packagesToList.ToArray()));

            }
            else
            {
                base.Out.Object.Write(TableRecords.CreatePackageRecordList());
            }



        }
    }
}
