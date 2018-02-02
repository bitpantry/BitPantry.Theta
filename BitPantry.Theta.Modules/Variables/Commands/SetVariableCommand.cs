using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Variables.Commands
{
    [Command("SetVariable")]
    [Alias("svar")]
    [Synopsis("Sets a variable")]
    class SetVariableCommand : InputCommand
    {
        [Parameter(ordinalPosition:1, autoCompleteValuesFunction:"GetVariableNamesAutoComplete", validationFunction:"ValidateVariableName")]
        [Synopsis("The name of the variable to set")]
        public string Name { get; set; }

        [Parameter(ordinalPosition:2)]
        [Synopsis("The value to set the variable to")]
        public string Value { get; set; }

        [Parameter(isRequired: false, autoCompleteValuesFunction: "GetContextNamesAutoComplete")]
        [Synopsis("The context to set the variable for. If this is absent, the variable is applied to the current context")]
        public string Context { get; set; }

        [Switch]
        [Synopsis("If this switch is present, the parameter will be set accross all contexts")]
        public SwitchParameter AllContexts { get; set; }

        public void ValidateVariableName(ValidationFunctionContext context)
        {
            if (context.Value.Length > 20)
                context.IsValid = false;

            if (!context.IsValid)
                context.Message = "The variable name must be less than 21 characters in length.";
        }

        public override void Invoke(CommandInvocationContext context)
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
                else // try to create and / or use the specified context
                {
                    ctx = VariableContextLogic.Instance.VariableContextCollection.Contexts.FirstOrDefault(c => c.Name.Equals(this.Context, StringComparison.OrdinalIgnoreCase));
                    if (ctx == null && base.Confirm(string.Format("The context '{0}' does not exist and will be created", this.Context), Component.ConfirmationResult.Yes))
                    {
                        ctx = new VariableContext() { Name = this.Context };
                        VariableContextLogic.Instance.VariableContextCollection.Contexts.Add(ctx);
                        VariableContextLogic.Instance.Save();
                    }

                    if (ctx == null)
                    {
                        base.Out.Error.WriteLine("The variable could not be set because the context was not created");
                        return;
                    }
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
                        base.Out.Verbose.WriteLine(string.Format("Updating variable '{0}' for context '{1}' with value '{2}'"
                            , this.Name, ctxToApply.Name, this.Value));
                    else
                        base.Out.Verbose.WriteLine(string.Format("Updating variable '{0}' with value '{1}'"
                            , this.Name, this.Value));

                    ctxToApply.Variables.FirstOrDefault(v => v.Name.Equals(this.Name)).Value = this.Value;
                }
                else
                {
                    if (this.AllContexts.IsPresent)
                        base.Out.Verbose.WriteLine(string.Format("Creating variable '{0}' for context '{1}' with value '{2}'"
                            , this.Name, ctxToApply.Name, this.Value));
                    else
                        base.Out.Verbose.WriteLine(string.Format("Creating variable '{0}' with value '{1}"
                            , this.Name, this.Value));

                    ctxToApply.Variables.Add(new VariableContextVariable()
                    {
                        Name = this.Name,
                        Value = this.Value
                    });
                }
            }

            VariableContextLogic.Instance.Save();

            base.Out.Object.Table(TableRecords.CreateVariableRecordList(new VariableContextVariable()
            {
                Name = this.Name,
                Value = this.Value
            }));
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
