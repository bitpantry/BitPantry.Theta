﻿using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Variables.Commands
{
    [Command("GetVariable")]
    [Alias("gvar")]
    [Synopsis("Gets the value of the given variable")]
    class GetVariableCommand : InputCommand
    {
        [Parameter(ordinalPosition: 1, autoCompleteValuesFunction: "GetVariableNamesAutoComplete")]
        [Synopsis("The name of the variable to get the value for")]
        public string Name { get; set; }

        [Parameter(isRequired: false, autoCompleteValuesFunction: "GetContextNamesAutoComplete", useAutoCompleteForValidation: true)]
        [Synopsis("The context to get the variable for. If this is absent, the variable is applied to the current context")]
        public string Context { get; set; }

        [Switch]
        [Synopsis("If this switch is present, the parameter will be retrieved accross all contexts")]
        public SwitchParameter AllContexts { get; set; }

        public override void Invoke(CommandInvocationContext context)
        {
            var contextsToApply = new List<VariableContext>();

            if (!AllContexts.IsPresent) // apply to specified or default context
            {
                VariableContext ctx = null;
                if (string.IsNullOrEmpty(Context)) // try to use the current context
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
                    ctx = VariableContextLogic.Instance.VariableContextCollection.Contexts.FirstOrDefault(c => c.Name.Equals(Context, StringComparison.OrdinalIgnoreCase));
                }

                contextsToApply.Add(ctx);
            }
            else // load all to apply to
            {
                base.Out.Verbose.WriteLine("Applying variable to all contexts");
                contextsToApply.AddRange(VariableContextLogic.Instance.VariableContextCollection.Contexts);
            }

            // apply to contexts

            var records = new List<VariableContextVariable>();

            foreach (var ctxToApply in contextsToApply)
            {
                var rec = ctxToApply.Variables.FirstOrDefault(v => v.Name.Equals(Name));
                if (rec != null)
                    records.Add(rec);
            }

            base.Out.Object.Table(TableRecords.CreateVariableRecordList(records.ToArray()));
        }

        public void GetContextNamesAutoComplete(AutoCompleteValuesFunctionContext context)
        {
            context.Values.AddRange(VariableContextLogic.Instance.VariableContextCollection.Contexts.Select(c => c.Name).ToList());
        }

        public void GetVariableNamesAutoComplete(AutoCompleteValuesFunctionContext context)
        {
            if (AllContexts.IsPresent)
            {
                context.Values.AddRange(VariableContextLogic.Instance.VariableContextCollection.Contexts
                    .SelectMany(c => c.Variables.Select(v => v.Name)).Distinct().ToList());
            }
            else
            {
                var ctx = VariableContextLogic.Instance.CurrentContext;
                if (ctx == null && !string.IsNullOrWhiteSpace(Context))
                    ctx = VariableContextLogic.Instance.VariableContextCollection.Contexts.FirstOrDefault(c => c.Name.Equals(Context, StringComparison.OrdinalIgnoreCase));

                if (ctx == null)
                    return;

                context.Values.AddRange(ctx.Variables.Select(v => v.Name).ToList());
            }
        }

    }
}
