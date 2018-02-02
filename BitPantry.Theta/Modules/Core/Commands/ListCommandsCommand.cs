using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Core.Commands
{
    [Command("ListCommands")]
    [Alias("lc")]
    [Synopsis("Lists all registered commands")]
    class ListCommandsCommand : InputCommand
    {
        [Parameter(ordinalPosition:1, isRequired:false)]
        [Synopsis("The filter to apply to the list to limit the results")]
        public string Filter { get; set; }

        [Parameter(isRequired:false, AutoCompleteValuesFunction="GetModuleNameAutoComplete", UseAutoCompleteForValidation=true)]
        [Synopsis("Lists commands for the given module")]
        public string Module { get; set; }

        [Switch]
        [Synopsis("When this switch is present, full command details are listed")]
        public SwitchParameter Full { get; set; }

        public override void Invoke(CommandInvocationContext context)
        {
            var commands = base.Host.Commands.ToList();
            if (!string.IsNullOrWhiteSpace(this.Filter))
                commands = commands.Where(c => c.CommandName.ToUpper().Contains(this.Filter.ToUpper())).ToList();

            List<dynamic> commandData = new List<dynamic>();

            foreach (var command in commands)
            {
                var module = base.Host.Modules.Where(m => m.CommandTypes.Contains(command.InputCommandType)).FirstOrDefault();

                bool add = true;
                if (!string.IsNullOrEmpty(this.Module) && !module.Name.Equals(this.Module))
                    add = false;

                if (add)
                {
                    if (this.Full.IsPresent)
                    {
                        commandData.Add(new
                        {
                            Name = command.CommandName,
                            Aliases = string.Join(", ", command.Aliases),
                            Module = module == null ? string.Empty : module.Name,
                            Synopsis = command.Synopsis ?? string.Empty,
                            @Type = command.InputCommandType.FullName,
                            @Assembly = string.Format("{0}{1}{2}",
                                command.InputCommandType.Assembly,
                                Environment.NewLine,
                                command.InputCommandType.Assembly.CodeBase.Replace("file:///", string.Empty))
                        });
                    }
                    else
                    {
                        commandData.Add(new
                        {
                            Name = command.CommandName,
                            Aliases = string.Join(", ", command.Aliases),
                            Module = module == null ? string.Empty : module.Name,
                            Synopsis = command.Synopsis ?? string.Empty
                        });
                    }
                }

            }

            base.Out.Object.Table(commandData);
        }

        public void GetModuleNameAutoComplete(AutoCompleteValuesFunctionContext context)
        {
            context.Values.AddRange(base.Host.Modules.Select(m => m.Name).ToList());
        }
    }
}
