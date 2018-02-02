using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Variables.Commands
{
    [Command("ListVariableContexts")]
    [Alias("lvc")]
    [Synopsis("Lists the available variable contexts")]
    class ListVariableContextsCommand : InputCommand
    {
        [Parameter(ordinalPosition: 1, isRequired: false)]
        [Synopsis("Filters the list of available environments")]
        public string Filter { get; set; }

        public override void Invoke(CommandInvocationContext context)
        {
            if (VariableContextLogic.Instance.VariableContextCollection.Contexts.Count > 0)
            {
                var contextsToList = new List<VariableContext>(VariableContextLogic.Instance.VariableContextCollection.Contexts);

                if (!string.IsNullOrWhiteSpace(this.Filter))
                    contextsToList = contextsToList.Where(e => e.Name.IndexOf(this.Filter, StringComparison.OrdinalIgnoreCase) > -1).ToList();

                base.Out.Object.Write(TableRecords.CreateVariableContextRecordList(contextsToList.ToArray()));

            }
            else
            {
                base.Out.Object.Write(TableRecords.CreateVariableContextRecordList());
            }
        }
    }
}
