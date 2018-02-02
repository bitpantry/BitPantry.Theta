using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Variables.Commands
{
    [Command("ListVariables")]
    [Alias("lv")]
    [Synopsis("Lists the variables for the given context")]
    class ListVariablesCommand : InputCommand
    {
        [Parameter(ordinalPosition:1, isRequired: false, autoCompleteValuesFunction: "GetContextNamesAutoComplete", useAutoCompleteForValidation:true)]
        [Synopsis("The context to list variables for. If this is absent, the variables from the current context will be listed")]
        public string Context { get; set; }

        public void GetContextNamesAutoComplete(AutoCompleteValuesFunctionContext context)
        {
            context.Values.AddRange(VariableContextLogic.Instance.VariableContextCollection.Contexts.Select(c => c.Name).ToList());
        }

        public override void Invoke(CommandInvocationContext context)
        {
            if (string.IsNullOrEmpty(this.Context) && VariableContextLogic.Instance.CurrentContext == null)
            {
                base.Out.Error.WriteLine("There is no context set or specified - please specify or set a context first");
            }
            else
            {
                var ctx = VariableContextLogic.Instance.CurrentContext;
                if (!string.IsNullOrWhiteSpace(this.Context))
                    ctx = VariableContextLogic.Instance.VariableContextCollection.Contexts.FirstOrDefault(c => c.Name.Equals(this.Context, StringComparison.OrdinalIgnoreCase));

                base.Out.Object.Table(TableRecords.CreateVariableRecordList(ctx.Variables.ToArray()));
            }
        }
    }
}
