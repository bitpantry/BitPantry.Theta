using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Variables.Commands
{
    [Command("CreateVariableContext")]
    [Synopsis("Creates a new variable context")]
    class CreateVariableContextCommand : InputCommand
    {
        [Parameter(ordinalPosition:1, validationFunction:"ValidateContextName")]
        [Synopsis("The name of the variable context to create")]
        public string Name { get; set; }

        [Parameter(ordinalPosition:2, isRequired:false)]
        [Synopsis("An optional description for the variable context")]
        public string Description { get; set; }

        public override void Invoke(CommandInvocationContext context)
        {
            var ctx = new VariableContext()
            {
                Name = Name,
                Description = Description
            };

            VariableContextLogic.Instance.VariableContextCollection.Contexts.Add(ctx);
            VariableContextLogic.Instance.Save();

            base.Out.Object.Table(TableRecords.CreateVariableContextRecordList(ctx));   
        }

        public void ValidateContextName(ValidationFunctionContext context)
        {
            if (VariableContextLogic.Instance.VariableContextCollection.Contexts.Any(c => c.Name.Equals(context.Value, StringComparison.OrdinalIgnoreCase)))
            {
                context.IsValid = false;
                context.Message = string.Format("The context name '{0}' is already being used"
                    , context.Value);
            }
        }
    }
}
