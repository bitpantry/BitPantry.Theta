using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Variables.Commands
{
    [Command("DeleteVariable")]
    [Synopsis("Deletes a variable")]
    [Alias("dvar")]
    class DeleteVariableCommand : InputCommand
    {
        [Parameter(ordinalPosition: 1, autoCompleteValuesFunction: "GetVariableNamesAutoComplete", useAutoCompleteForValidation:true)]
        [Synopsis("The name of the variable to delete")]
        public string Name { get; set; }

        [Parameter(isRequired: false, autoCompleteValuesFunction: "GetContextNamesAutoComplete", useAutoCompleteForValidation:true)]
        [Synopsis("The context to delete the variable for. If this is absent, the variable is applied to the current context")]
        public string Context { get; set; }

        [Switch]
        [Synopsis("If this switch is present, the parameter will be deleted accross all contexts")]
        public SwitchParameter AllContexts { get; set; }

        public override void Invoke(CommandInvocationContext context)
        {
            if (base.Confirm(string.Format("The variable '{0}' will be deleted - this cannot be undone", this.Name), Component.ConfirmationResult.Yes))
            {
                var contextsToApply = new List<VariableContext>();

                if (!this.AllContexts.IsPresent) // apply to specified or default context
                {
                    VariableContext ctx = null;
                    if (string.IsNullOrEmpty(this.Context)) // try to use the current context
                    {
                        if (VariableContextLogic.Instance.CurrentContext == null)
                        {
                            base.Out.Error.WriteLine("There is no context currently loaded and no context specified. Please load a context or specify a context using the 'Context' parameter first.");
                            return;
                        }

                        ctx = VariableContextLogic.Instance.CurrentContext;
                        base.Out.Verbose.WriteLine(string.Format("No context was specified - using the current context '{0}'", ctx.Name));
                    }
                    else // try to or use the specified context
                    {
                        ctx = VariableContextLogic.Instance.VariableContextCollection.Contexts.FirstOrDefault(c => c.Name.Equals(this.Context, StringComparison.OrdinalIgnoreCase));
                    }

                    contextsToApply.Add(ctx);
                }
                else // load all to apply to
                {
                    base.Out.Verbose.WriteLine("Applying variable to all contexts");
                    contextsToApply.AddRange(VariableContextLogic.Instance.VariableContextCollection.Contexts);
                }

                // apply to contexts

                foreach (var ctxToApply in contextsToApply)
                {
                    if (ctxToApply.Variables.Any(v => v.Name.Equals(this.Name)))
                    {
                        if (this.AllContexts.IsPresent)
                            base.Out.Verbose.WriteLine(string.Format("Deleting variable '{0}' for context '{1}'"
                                , this.Name, ctxToApply.Name));
                        else
                            base.Out.Verbose.WriteLine(string.Format("Deleting variable '{0}'"
                                , this.Name));

                        ctxToApply.Variables.Remove(ctxToApply.Variables.FirstOrDefault(v => v.Name.Equals(this.Name)));
                    }
                }

                VariableContextLogic.Instance.Save();
            }
            else
            {
                base.Out.Standard.WriteLine("The variable was NOT deleted.");
            }
        }

        public void GetContextNamesAutoComplete(AutoCompleteValuesFunctionContext context)
        {
            context.Values.AddRange(VariableContextLogic.Instance.VariableContextCollection.Contexts.Select(c => c.Name).ToList());
        }

        public void GetVariableNamesAutoComplete(AutoCompleteValuesFunctionContext context)
        {
            if (this.AllContexts.IsPresent)
            {
                context.Values.AddRange(VariableContextLogic.Instance.VariableContextCollection.Contexts
                    .SelectMany(c => c.Variables.Select(v => v.Name)).Distinct().ToList());
            }
            else
            {
                var ctx = VariableContextLogic.Instance.CurrentContext;
                if (ctx == null && !string.IsNullOrWhiteSpace(this.Context))
                    ctx = VariableContextLogic.Instance.VariableContextCollection.Contexts.FirstOrDefault(c => c.Name.Equals(this.Context, StringComparison.OrdinalIgnoreCase));

                if (ctx == null)
                    return;

                context.Values.AddRange(ctx.Variables.Select(v => v.Name).ToList());
            }
        }
    }
}
