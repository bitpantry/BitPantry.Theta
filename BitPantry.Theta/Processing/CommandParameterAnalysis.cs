using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPantry.Theta.Extensions;

namespace BitPantry.Theta.Processing
{
    class CommandParameterAnalysis
    {
        public IEnumerable<ParameterDef> ParametersInSet { get; set; }
        public List<ParameterDef> AvailableParameters { get; set; }
        public List<ParameterDef> MissingRequiredParameters { get; set; }
        public List<ParameterDef> SuperflousParameters { get; set; }
        public List<SwitchDef> AvaliableSwitches { get; set; }

        public CommandParameterAnalysis(InputCommand command)
        {
            // get defintion

            var definition = command.GetType().DescribeInputCommand();

            // get parameters in set - all parameters available for the active set

            ParametersInSet =  definition.Parameters;
            if(!string.IsNullOrWhiteSpace(command.ActiveParameterSet))
                ParametersInSet = ParametersInSet
                    .Where(p => string.IsNullOrEmpty(p.ParameterSet)
                    || p.ParameterSet.Equals(command.ActiveParameterSet)).ToList();

            // get missing required parameters in set - where a complete parameter node is missing for a required parameter in the current set

            MissingRequiredParameters = ParametersInSet.Where(p => p.IsRequired
                && p.PropertyInfo.GetValue(command) == null).ToList();

            // get superflous parameters - where at least a named parameter switch is defined for a parameter that is not included in the current set

            SuperflousParameters = definition.Parameters.Where(p => p.PropertyInfo.GetValue(command) != null
                && !ParametersInSet.Contains(p)).ToList();

            // get available parameters in set - all remaining parameters where at least a named parameter switch has not been defined for the current set

            AvailableParameters = definition.Parameters.Where(p => p.PropertyInfo.GetValue(command) == null
                && ParametersInSet.Contains(p)).ToList();

            // get available switches

            AvaliableSwitches = definition.Switches.Where(s => !((SwitchParameter)s.PropertyInfo.GetValue(command)).IsPresent).ToList();
        }

        public CommandParameterAnalysis(CommandResolver resolver)
        {
            // get parameters in set - all parameters available for the active set

            ParametersInSet = resolver.CommandDefinition.Parameters;
            if(!string.IsNullOrEmpty(resolver.ActiveParameterSet))
                ParametersInSet = ParametersInSet
                    .Where(p => string.IsNullOrEmpty(p.ParameterSet) 
                    || p.ParameterSet.Equals(resolver.ActiveParameterSet)).ToList();

            // get missing required parameters in set - where a complete parameter node is missing for a required parameter in the current set

            List<ParameterDef> completedParameters = resolver.Nodes.Where(n => n.Parameter != null
                && n.IsComplete).Select(n => n.Parameter).ToList();

            MissingRequiredParameters = ParametersInSet.Where(p => p.IsRequired
                && !completedParameters.Contains(p)).ToList();

            // get superflous parameters - where at least a named parameter switch is defined for a parameter that is not included in the current set

            List<ParameterDef> parameters = resolver.Nodes.Where(n => n.Parameter != null).Select(n => n.Parameter).ToList();
            SuperflousParameters = parameters.Where(p => !ParametersInSet.Contains(p)).ToList();

            // get available parameters in set - all remaining parameters where at least a named parameter switch has not been defined for the current set

            AvailableParameters = ParametersInSet.Where(p => !parameters.Contains(p)).ToList();

            // get available switches

            AvaliableSwitches = resolver.CommandDefinition.Switches
                .Where(s => !resolver.Nodes.Where(n => n.Type == CRNodeType.Switch).Select(n => n.Switch).Contains(s))
                .ToList();
          
        }
    }
    
}
