using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPantry.Theta.API;

namespace BitPantry.Theta.Modules.Packages.Commands
{
    [Command("CreatePackage")]
    [Synopsis("Creates a new package")]
    class CreatePackageCommand : InputCommand
    {
        [Parameter(ordinalPosition:1)]
        [Synopsis("The unique name of the package")]
        public string Name { get; set; }

        public override void Invoke(CommandInvocationContext context)
        {
            if (PackageLogic.Instance.PackagesCollection.Packages
                .Exists(e => e.Name.Equals(Name, StringComparison.OrdinalIgnoreCase)))
            {
                base.Out.Warning.WriteLine(string.Format("The package name '{0}' is already being used by another package", Name));
            }
            else
            {
                var newPkg = new Packages.Package()
                {
                    Name = Name
                };

                PackageLogic.Instance.PackagesCollection.Packages.Add(newPkg);
                PackageLogic.Instance.Save();

                base.Out.Object.Write(TableRecords.CreatePackageRecordList(newPkg));               
            }
        }
    }
}
