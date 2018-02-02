using BitPantry.Theta.API;
using BitPantry.Theta.Component;
using BitPantry.Theta.Component.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Core.Commands
{
    [Command("ListAssemblyModules")]
    [Synopsis("Lists all of the available assemblies in a given module")]
    class ListAssemblyModulesCommand : InputCommand
    {
        [Parameter(ordinalPosition: 1, autoCompleteValuesFunction: "GetAssemblyFilenameAutoCompleteValues")]
        [Synopsis("The name of a globally located assembly or the file path to the assembly for which to install all modules")]
        public string Assembly { get; set; }

        public override void Invoke(CommandInvocationContext context)
        {
            base.Out.Object.Table(TableRecords.CreateModuleTableRecordList(false, AssemblyUtil.GetAssemblyModules(Assembly).ToArray()));
        }

        public void GetAssemblyFilenameAutoCompleteValues(AutoCompleteValuesFunctionContext context)
        {
            context.Values.AddRange(new UserAssemblyRepository().Assemblies.Select(a => a.Name));
        }
    }
}
