using BitPantry.Theta.API;
using BitPantry.Theta.Modules.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Core
{
    class Module : IModule
    {
        public string Name
        {
            get { return "Core"; }
        }

        public List<Type> Dependencies
        {
            get { return null; }
        }

        public List<Type> CommandTypes
        {
            get
            {
                return new List<Type>()
                {
                    typeof(ClearCommand),
                    typeof(HelpCommand),
                    typeof(ListModulesCommand),
                    typeof(InstallModuleCommand),
                    typeof(UninstallModuleCommand),
                    typeof(ListCommandsCommand),
                    typeof(ListAssemblyModulesCommand),
                    typeof(OpenUserAssembliesDirectoryCommand)
                };
            }
        }

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
