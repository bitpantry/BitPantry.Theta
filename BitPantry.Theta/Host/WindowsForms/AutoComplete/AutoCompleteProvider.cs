using BitPantry.Theta.API;
using BitPantry.Theta.Component;
using BitPantry.Theta.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Host.WindowsForms.AutoComplete
{
    class AutoCompleteProvider
    {
        enum SwitchDefinitionType
        {
            Parameter,
            Switch
        }

        private IHostInterface _hostInterface = null;

        public AutoCompleteProvider(IHostInterface hostInterface) 
        {
            this._hostInterface = hostInterface;
        }

        public List<string> GetOptions(Input input, InputNode currentNode)
        {
            CommandResolver resolver = new CommandResolver(input, this._hostInterface.Commands);

            if (string.IsNullOrWhiteSpace(input.ToString()) || currentNode.ElementType == InputNodeType.Command) // node is in leading white space or at very beginning of command or is the command node
                return this.GetCommandsAutoComplete(input, currentNode);
            else if (resolver.CommandDefinition != null && currentNode.ElementType == InputNodeType.NamedParameter)
                return this.GetNamedParameterAutoComplete(resolver, currentNode);
            else if (resolver.CommandDefinition != null && currentNode.ElementType == InputNodeType.NamedParameterValue)
                return this.GetParameterAutoComplete(resolver, currentNode);
            else if (resolver.CommandDefinition != null && currentNode.ElementType == InputNodeType.Empty)
                return this.GetParameterAutoComplete(resolver, currentNode);
            else if (resolver.CommandDefinition != null && currentNode.ElementType == InputNodeType.OrdinalParameterValue)
                return this.GetParameterAutoComplete(resolver, currentNode);
            else if (resolver.CommandDefinition != null && currentNode.ElementType == InputNodeType.Switch)
                return this.GetSwitchAutoComplete(resolver, currentNode);

            return null; 
        }

        private List<string> GetParameterAutoComplete(CommandResolver resolver, InputNode currentNode)
        {
            CommandResolverNode resolverNode = resolver.GetNode(currentNode);
            ParameterDef def = resolverNode != null ? resolverNode.Parameter : null;

            string validationFunctionName = null;

            // try to find ordinal parameter definition for an empty element

            if (def == null && currentNode.ElementType == InputNodeType.Empty)
            {
                int ordinalIndex = resolver.Input.InputNodes
                    .Where(n => n.Index < currentNode.Index && n.ElementType == InputNodeType.OrdinalParameterValue).Count() + 1;
                
                def = new CommandParameterAnalysis(resolver).AvailableParameters
                    .Where(p => p.OrdinalPosition == ordinalIndex).FirstOrDefault();
            }

            // find validation function for a valid resolver node

            if (def != null)
                validationFunctionName = def.AutoCompleteValuesFunction;
            
            // execute validation values function

            try
            {
                if (validationFunctionName != null)
                    return Util.GetParameterAutoCompleteValues(resolver, def, this._hostInterface)
                    .Where(v => v.StartsWith(currentNode.Value, StringComparison.OrdinalIgnoreCase)).ToList();

            }
            catch (Exception ex)
            {
                // TODO: Do something with this exception
            }

            return null;
        }

        private List<string> GetSwitchAutoComplete(CommandResolver resolver, InputNode currentNode)
        {
            CommandParameterAnalysis analysis = new CommandParameterAnalysis(resolver);

            List<string> values = analysis.AvaliableSwitches
                .Where(s => s.Name.StartsWith(currentNode.Value, StringComparison.OrdinalIgnoreCase))
                .Select(s => s.Name).ToList();

            values.AddRange(analysis.AvaliableSwitches
                .SelectMany(s => s.Aliases)
                .Where(a => a.StartsWith(currentNode.Value, StringComparison.OrdinalIgnoreCase)));

            return values.OrderBy(v => v).ToList();
        }

        private List<string> GetNamedParameterAutoComplete(CommandResolver resolver, InputNode currentNode)
        {
            CommandParameterAnalysis analysis = new CommandParameterAnalysis(resolver);

            List<string> values = analysis.AvailableParameters
                .Where(p => p.Name.StartsWith(currentNode.Value, StringComparison.OrdinalIgnoreCase))
                .Select(p => p.Name).ToList();

            values.AddRange(analysis.AvailableParameters
                .SelectMany(p => p.Aliases)
                .Where(a => a.StartsWith(currentNode.Value, StringComparison.OrdinalIgnoreCase)));

            return values.OrderBy(v => v).ToList();            
        }

        private List<string> GetCommandsAutoComplete(Input input, InputNode node)
        {
            if (node == null)
                return this.GetAvailableCommands();
            else
                return this.GetAvailableCommands().Where(c => c.StartsWith(node.Value, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private List<string> GetAvailableCommands()
        {
            List<string> options = new List<string>();

            options.AddRange(this._hostInterface.Commands.Select(c => c.CommandName).ToList());
            options.AddRange(this._hostInterface.Commands.SelectMany(d => d.Aliases).ToList());

            return options.OrderBy(o => o).ToList();
        }

    }
}
