using BitPantry.Theta.API;
using BitPantry.Theta.Component;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BitPantry.Theta.Processing
{
    class CommandResolver
    {
        public Input Input { get; set; }
        public CommandCollection AvailableCommands { get; set; }

        public InputCommandDef CommandDefinition { get; set; }
        public string ActiveParameterSet { get; private set; }

        public List<CommandResolverNode> Nodes { get; set; }

        public bool HasErrors
        {
            get
            {
                return Nodes.Exists(n => !string.IsNullOrWhiteSpace(n.ResolutionError))
                    || CommandDefinition == null;
            }
        }

        public List<string> Errors
        {
            get
            {
                List<string> errors = new List<string>();
                if (CommandDefinition == null)
                    errors.Add("The input could not be resolved to a registered command");
                errors.AddRange(Nodes.Where(n => !string.IsNullOrWhiteSpace(n.ResolutionError)).Select(n => n.ResolutionError).ToList());
                return errors;
            }
        }

        public CommandResolver(Input input, CommandCollection availableCommands)
        {
            // initialize

            Input = input;
            AvailableCommands = availableCommands;
            Nodes = new List<CommandResolverNode>();

            // resolve command

            CommandDefinition = Util.GetCommand(AvailableCommands, Input);
            if (CommandDefinition != null)
            {
                // resolve input nodes

                ResolveInputNodes();

                // derive parameter set

                ActiveParameterSet = Nodes.Where(n => n.Parameter != null
                    && !string.IsNullOrEmpty(n.Parameter.ParameterSet))
                    .Select(n => n.Parameter.ParameterSet)
                    .FirstOrDefault();
            }
        }

        private void ResolveInputNodes()
        {
            int ordinalParameterIndex = 1;

            foreach (var node in Input.InputNodes)
            {
                switch (node.ElementType)
                {
                    case InputNodeType.NamedParameter:
                        ResolveNamedParameter(node);
                        break;
                    case InputNodeType.Switch:
                        ResolveSwitch(node);
                        break;
                    case InputNodeType.OrdinalParameterValue:
                        if (ResolveOrdinalParameter(node, ordinalParameterIndex))
                            ordinalParameterIndex++;
                        break;
                }
            }
        }

        private void ResolveNamedParameter(InputNode node)
        {
            // get parameter definition - add resolution error and exit if not found

            ParameterDef param = GetParameterDefinition(node.Value);

            if (param == null) // parameter is not defined in defintion
            {
                Nodes.Add(new CommandResolverNode()
                {
                    Type = CRNodeType.NamedParameter,
                    IsComplete = false,
                    InputNode = node,
                    ResolutionError = string.Format("Parameter '{0}' is undefined", node.Value)
                });
            }
            else if (Nodes.Exists(n => n.Parameter == param)) // parameter is already specified in input
            {
                Nodes.Add(new CommandResolverNode()
                {
                    Type = CRNodeType.NamedParameter,
                    IsComplete = false,
                    InputNode = node,
                    ResolutionError = string.Format("Parameter '{0}' is already defined", node.Value)
                });
            }
            else // resolve node
            {
                Nodes.Add(new CommandResolverNode()
                {
                    Type = CRNodeType.NamedParameter,
                    IsComplete = node.IsPairedWith != null,
                    InputNode = node,
                    Parameter = param,
                });
            }
        }

        private void ResolveSwitch(InputNode node)
        {
            // get switch definition = add resolution error and exit if not found

            SwitchDef def = GetSwitchDefinition(node.Value);

            if (def == null) // switch is not defined in current definition
            {
                Nodes.Add(new CommandResolverNode()
                {
                    Type = CRNodeType.Switch,
                    IsComplete = false,
                    InputNode = node,
                    ResolutionError = string.Format("Switch '{0}' is undefined", node.Value)
                });
            }
            else if (Nodes.Exists(n => n.Switch == def)) // build resolver node if not already defined
            {
                Nodes.Add(new CommandResolverNode()
                {
                    Type = CRNodeType.Switch,
                    IsComplete = false,
                    InputNode = node,
                    ResolutionError = string.Format("Switch '{0}' is defined more than once", node.Value)
                });
            }
            else // resolve node
            {
                Nodes.Add(new CommandResolverNode()
                {
                    Type = CRNodeType.Switch,
                    IsComplete = true,
                    InputNode = node,
                    Switch = def
                });
            }
        }

        private bool ResolveOrdinalParameter(InputNode node, int index)
        {
            // locate ordinal index

            ParameterDef param = CommandDefinition.Parameters.FirstOrDefault(p => p.OrdinalPosition == index);

            if (param == null) // ordinal parameter is not defined for current defintion
            {
                Nodes.Add(new CommandResolverNode()
                {
                    Type = CRNodeType.OrdinalParameter,
                    IsComplete = false,
                    InputNode = node,
                    ResolutionError = string.Format("An ordinal parameter at position {0} could not be found for ordinal parameter value '{1}'", index, node.Value)
                });
                
                return false;
            }
            else
            {
                Nodes.Add(new CommandResolverNode()
                {
                    Type = CRNodeType.OrdinalParameter,
                    IsComplete = true,
                    InputNode = node,
                    Parameter = param
                });

                return true;
            }
        }

        public CommandResolverNode GetNode(InputNode node)
        {
            if (node.ElementType == InputNodeType.NamedParameter)
            {
                return Nodes.FirstOrDefault(n => n.Parameter != null && n.InputNode == node);
            }
            else if (node.ElementType == InputNodeType.NamedParameterValue)
            {
                return Nodes.FirstOrDefault(n => n.Parameter != null && n.InputNode.IsPairedWith == node);
            }
            else if (node.ElementType == InputNodeType.OrdinalParameterValue)
            {
                var ordinalParameterInputNodes = Input.InputNodes.Where(n => n.ElementType == InputNodeType.OrdinalParameterValue).ToList();
                int ordinalIndex = ordinalParameterInputNodes.IndexOf(node) + 1;

                return Nodes
                    .Where(n => n.Parameter != null && n.Parameter.OrdinalPosition == ordinalIndex)
                    .FirstOrDefault();
            }
            else if (node.ElementType == InputNodeType.Switch)
            {
                return Nodes.FirstOrDefault(n => n.Switch != null && n.InputNode == node);
            }
            else if (node.ElementType == InputNodeType.Empty)
            {
                // try to find preceeding named parameter node - cannot find ordinal here because a node is not created for ordinals that don't yet exist

                if (node.Index > 0)
                    return Nodes
                        .FirstOrDefault(n => n.InputNode.Index == node.Index - 1 && n.Type == CRNodeType.NamedParameter);
            }

            return null;
        }

        #region DEFINITION HELPERS

        private ParameterDef GetParameterDefinition(string parameterName)
        {
            return CommandDefinition.Parameters.FirstOrDefault(p => p.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase)
                || p.Aliases.Contains(parameterName, StringComparer.OrdinalIgnoreCase));
        }

        private SwitchDef GetSwitchDefinition(string switchName)
        {
            return CommandDefinition.Switches.FirstOrDefault(p => p.Name.Equals(switchName, StringComparison.OrdinalIgnoreCase)
                || p.Aliases.Contains(switchName, StringComparer.OrdinalIgnoreCase));
        }

        #endregion
    }
}
