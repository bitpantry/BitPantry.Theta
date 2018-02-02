using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Variables.Commands
{
    [Command("DeleteVariableContext")]
    [Synopsis("Deletes the given variable context, including all associated variables - this cannot be undone")]
    class DeleteVariableContextCommand : InputCommand
    {
        [Parameter(ordinalPosition:1, autoCompleteValuesFunction:"GetContextNameAutoComplete", useAutoCompleteForValidation:true)]
        [Synopsis("The name of the variable context to delete")]
        public string Name { get; set; }

        [Switch]
        [Synopsis("Forces the deleting on a loaded variable context")]
        public SwitchParameter Force { get; set; }

        public override void Invoke(CommandInvocationContext context)
        {

            var ctx = VariableContextLogic.Instance.VariableContextCollection.Contexts.FirstOrDefault(e => e.Name.Equals(this.Name, StringComparison.OrdinalIgnoreCase));

            if (VariableContextLogic.Instance.CurrentContext == ctx && !this.Force.IsPresent)
            {
                base.Out.Warning.WriteLine("The context is loaded. Unload the context, or use the 'Force' switch");
            }
            else
            {
                if (base.Confirm("The context along with all associated variables will be deleted. This cannot be undone.", Component.ConfirmationResult.No))
                {
                    VariableContextLogic.Instance.VariableContextCollection.Contexts.Remove(ctx);
                    VariableContextLogic.Instance.Save();

                    base.Out.Standard.WriteLine("Variable context deleted.");
                }
                else
                {
                    base.Out.Standard.WriteLine("The variable context has NOT been deleted.");
                }
            }
            
        }

        public void GetContextNameAutoComplete(AutoCompleteValuesFunctionContext context)
        {
            context.Values.AddRange(VariableContextLogic.Instance.VariableContextCollection.Contexts.Select(e => e.Name).ToList());
        }
    }
}
