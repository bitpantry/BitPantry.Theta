using BitPantry.Theta.API;
using BitPantry.Theta.Component;
using BitPantry.Theta.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Processing
{
    class CommandActivator
    {
        public CommandActivator() { }

        public InputCommand Create(CommandResolver resolver, IHostInterface host)
        {
            // load command from resolver

            InputCommand cmd = this.CreateInputCommand(resolver, host);

            // set internal properties

            cmd.ActiveParameterSet = resolver.ActiveParameterSet;
            cmd.InstallInterfaceServices(host);

            return cmd;
        }

        private InputCommand CreateInputCommand(CommandResolver resolver, IHostInterface host)
        {
            // instantiate input command

            if (resolver.CommandDefinition == null)
                throw new InvalidOperationException("Cannot load input command because a command definition could not be resolved");

            InputCommand cmd = host.CommandActivatorContainer.Get(resolver.CommandDefinition.InputCommandType);

            // initialize switches to false

            foreach (var item in resolver.CommandDefinition.Switches)
                item.PropertyInfo.SetValue(cmd, new SwitchParameter() { IsPresent = false });

            // set node values

            foreach (var node in resolver.Nodes.Where(n => n.IsComplete))
                node.SetValue(cmd);

            return cmd;
        }
    }
}
