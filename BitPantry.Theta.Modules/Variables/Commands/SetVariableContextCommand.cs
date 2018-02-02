using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Variables.Commands
{
    [Command("SetVariableContext")]
    [Alias("setcon")]
    [Synopsis("Sets the current variable environment context")]
    class SetVariableContextCommand : InputCommand
    {
        [Parameter(ordinalPosition:1, autoCompleteValuesFunction: "GetContextNamesAutoComplete", useAutoCompleteForValidation:true)]
        [Synopsis("The context to set the variable for. If this is absent, the variable is applied to the current context")]
        public string Context { get; set; }

        public void GetContextNamesAutoComplete(AutoCompleteValuesFunctionContext context)
        {
            context.Values.AddRange(VariableContextLogic.Instance.VariableContextCollection.Contexts.Select(c => c.Name).ToList());
        }

        public override void Invoke(CommandInvocationContext context)
        {
            VariableContextLogic.Instance.CurrentContext 
                = VariableContextLogic.Instance.VariableContextCollection.Contexts
                .FirstOrDefault(c => c.Name.Equals(Context, StringComparison.OrdinalIgnoreCase));
            base.Out.Object.Table(TableRecords.CreateVariableContextRecordList(VariableContextLogic.Instance.CurrentContext));
        }
    }
}
