using BitPantry.Theta.API;
using BitPantry.Theta.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Core.Commands
{
    [Command("ListModules")]
    [Alias("lmod")]
    [Synopsis("Lists the details for the currently loaded modules")]
    class ListModulesCommand : InputCommand
    {
        private const string TAB = "  ";

        public override void Invoke(CommandInvocationContext context)
        {
            if (base.Host.Modules.Count > 0)
            {
                List<IModule> moduleList = new List<IModule>();
                foreach (var module in base.Host.Modules)
                    moduleList.Add(module);

                base.Out.Object.Table(Component.Modules.TableRecords.CreateModuleTableRecordList(moduleList.ToArray()));

            }
            else
            {
                base.Out.Object.Table(Component.Modules.TableRecords.CreateModuleTableRecordList());
            }
        }
    }
}
