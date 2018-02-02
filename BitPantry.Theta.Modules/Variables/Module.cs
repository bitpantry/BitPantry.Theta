using BitPantry.Theta.API;
using BitPantry.Theta.Modules.Variables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Variables
{
    public class Module : IModule
    {
        public string Name
        {
            get { return "Variables"; }
        }

        public List<Type> CommandTypes
        {
            get
            {
                return new List<Type>()
                {
                    typeof(ListVariableContextsCommand),
                    typeof(DeleteVariableCommand),
                    typeof(DeleteVariableContextCommand),
                    typeof(GetVariableCommand),
                    typeof(CreateVariableContextCommand),
                    typeof(SetVariableCommand),
                    typeof(SetVariableContextCommand),
                    typeof(ListVariablesCommand),
                    typeof(OpenVariableContextFileCommand)
                };
            }
        }

        public List<Type> Dependencies
        {
            get { return new List<Type>() { typeof(Packages.Module) }; }
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
