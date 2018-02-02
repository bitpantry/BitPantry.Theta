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
                return this.Nodes.Exists(n => !string.IsNullOrWhiteSpace(n.ResolutionError))
                    || this.CommandDefinition == null;
            }
        }

        public List<string> Errors
        {
            get
            {
                List<string> errors = new List<string>();
                if (this.CommandDefinition == null)
                    errors.Add("The input could not be resolved to a registered command");
                errors.AddRange(this.Nodes.Where(n => !string.IsNullOrWhiteSpace(n.ResolutionError)).Select(n => n.ResolutionError).ToList());
                return errors;
            }
        }

        public CommandResolver(Input input, CommandCollection availableCommands)
        {
            // initialize

            this.Input = input;
            this.AvailableCommands = availableCommands;
            this.Nodes = new List<CommandResolverNode>();

            // resolve command

            this.CommandDefinition = Util.GetCommand(this.AvailableCommands, this.Input);
            if (this.CommandDefinition != null)
            {
                // resolve input nodes

                this.ResolveInputNodes();

                // derive parameter set

                this.ActiveParameterSet = this.Nodes.Where(n => n.Parameter != null
                    && !string.IsNullOrEmpty(n.Parameter.ParameterSet))
                    .Select(n => n.Parameter.ParameterSet)
                    .FirstOrDefault();
            }
        }

        private void ResolveInputNodes()
        {
            int ordinalParameterIndex = 1;

            foreach (var node in this.Input.InputNodes)
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

            ParameterDef param = this.GetParameterDefinition(node.Value);

            if (param == null) // parameter is not defined in defintion
            {
                this.Nodes.Add(new CommandResolverNode()
                {
                    Type = CRNodeType.NamedParameter,
                    IsComplete = false,
                    InputNode = node,
                    ResolutionError = string.Format("Parameter '{0}' is undefined", node.Value)
                });
            }
            else if (this.Nodes.Exists(n => n.Parameter == param)) // parameter is already specified in input
            {
                this.Nodes.Add(new CommandResolverNode()
                {
                    Type = CRNodeType.NamedParameter,
                    IsComplete = false,
                    InputNode = node,
                    ResolutionError = string.Format("Parameter '{0}' is already defined", node.Value)
                });
            }
            else // resolve node
            {
                this.Nodes.Add(new CommandResolverNode()
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

            SwitchDef def = this.GetSwitchDefinition(node.Value);

            if (def == null) // switch is not defined in current definition
            {
                this.Nodes.Add(new CommandResolverNode()
                {
                    Type = CRNodeType.Switch,
                    IsComplete = false,
                    InputNode = node,
                    ResolutionError = string.Format("Switch '{0}' is undefined", node.Value)
                });
            }
            else if (this.Nodes.Exists(n => n.Switch == def)) // build resolver node if not already defined
            {
                this.Nodes.Add(new CommandResolverNode()
                {
                    Type = CRNodeType.Switch,
                    IsComplete = false,
                    InputNode = node,
                    ResolutionError = string.Format("Switch '{0}' is defined more than once", node.Value)
                });
            }
            else // resolve node
            {
                this.Nodes.Add(new CommandResolverNode()
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

            ParameterDef param = this.CommandDefinition.Parameters.FirstOrDefault(p => p.OrdinalPosition == index);

            if (param == null) // ordinal parameter is not defined for current defintion
            {
                this.Nodes.Add(new CommandResolverNode()
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
                this.Nodes.Add(new CommandResolverNode()
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
                return this.Nodes.FirstOrDefault(n => n.Parameter != null && n.InputNode == node);
            }
            else if (node.ElementType == InputNodeType.NamedParameterValue)
            {
                return this.Nodes.FirstOrDefault(n => n.Parameter != null && n.InputNode.IsPairedWith == node);
            }
            else if (node.ElementType == InputNodeType.OrdinalParameterValue)
            {
                var ordinalParameterInputNodes = this.Input.InputNodes.Where(n => n.ElementType == InputNodeType.OrdinalParameterValue).ToList();
                int ordinalIndex = ordinalParameterInputNodes.IndexOf(node) + 1;

                return this.Nodes
                    .Where(n => n.Parameter != null && n.Parameter.OrdinalPosition == ordinalIndex)
                    .FirstOrDefault();
            }
            else if (node.ElementType == InputNodeType.Switch)
            {
                return this.Nodes.FirstOrDefault(n => n.Switch != null && n.InputNode == node);
            }
            else if (node.ElementType == InputNodeType.Empty)
            {
                // try to find preceeding named parameter node - cannot find ordinal here because a node is not created for ordinals that don't yet exist

                if (node.Index > 0)
                    return this.Nodes
                        .FirstOrDefault(n => n.InputNode.Index == node.Index - 1 && n.Type == CRNodeType.NamedParameter);
            }

            return null;
        }

        #region DEFINITION HELPERS

        private ParameterDef GetParameterDefinition(string parameterName)
        {
            return this.CommandDefinition.Parameters.FirstOrDefault(p => p.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase)
                || p.Aliases.Contains(parameterName, StringComparer.OrdinalIgnoreCase));
        }

        private SwitchDef GetSwitchDefinition(string switchName)
        {
            return this.CommandDefinition.Switches.FirstOrDefault(p => p.Name.Equals(switchName, StringComparison.OrdinalIgnoreCase)
                || p.Aliases.Contains(switchName, StringComparer.OrdinalIgnoreCase));
        }

        #endregion
    }
}
