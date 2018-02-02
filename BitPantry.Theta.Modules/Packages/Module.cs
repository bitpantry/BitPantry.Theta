using BitPantry.Theta.API;
using BitPantry.Theta.Modules.Packages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Packages
{
    public class Module : IModule
    {
        public string Name { get { return "Packages"; } }

        public List<Type> CommandTypes
        {
            get
            {
                return new List<Type>()
                {
                    typeof(ListPackagesCommand),
                    typeof(CreatePackageCommand),
                    typeof(DeletePackageCommand),
                    typeof(LoadPackageCommand),
                    typeof(AddPackageModuleCommand),
                    typeof(ListPackageDetailsCommand),
                    typeof(RemovePackageModuleCommand),
                    typeof(UnloadPackageCommand),
                    typeof(OpenPackagesFileCommand)
                };
            }
        }

        public List<Type> Dependencies { get { return null; } }

        public void Install()
        {
            // do nothing
        }

        public void Uninstall()
        {
            // do nothing
        }
    }
}
