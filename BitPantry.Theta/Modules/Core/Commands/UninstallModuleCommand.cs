using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Core.Commands
{
    [Command("UninstallModule")]
    [Alias("umod")]
    [Synopsis("Uninstalls the specified module")]
    class UninstallModuleCommand : InputCommand
    {
        [Parameter(ordinalPosition:1, autoCompleteValuesFunction:"GetModuleNameAutoComplete", useAutoCompleteForValidation:true)]
        [Synopsis("The name of the module to install")]
        public string ModuleName { get; set; }

        public override void Invoke(CommandInvocationContext context)
        {
            if (base.Confirm(string.Format("The module '{0}' will be uninstalled", ModuleName)))
            {
                if (base.Host.Modules.Uninstall(ModuleName, base.Out))
                    base.Out.Standard.WriteLine(string.Format("Module '{0}' has been installed", ModuleName));
            }
        }

        public void GetModuleNameAutoComplete(AutoCompleteValuesFunctionContext context)
        {
            context.Values.AddRange(base.Host.Modules.Select(m => m.Name));
        }

    }
}
