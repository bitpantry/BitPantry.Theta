using System.Reflection;
using BitPantry.Theta.API;
using BitPantry.Theta.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPantry.Parsing.Strings;

namespace BitPantry.Theta.Processing
{
    enum CRNodeType
    {
        Undefined,
        NamedParameter,
        Switch,
        OrdinalParameter
    }

    class CommandResolverNode
    {
        public bool IsComplete { get; set; }
        public CRNodeType Type { get; set; }

        public InputNode InputNode { get; set; }

        public ParameterDef Parameter { get; set; }
        public SwitchDef Switch { get; set; }

        public string ResolutionError { get; set; }

        public void SetValue(InputCommand cmd)
        {
            if (!IsComplete)
                throw new InvalidOperationException("Command property cannot be set from incomplete command resolver node");

            if (Type == CRNodeType.NamedParameter)
                ParseAndSetPropertyValue(cmd, InputNode.IsPairedWith.Value);
            else if (Type == CRNodeType.OrdinalParameter)
                ParseAndSetPropertyValue(cmd, InputNode.Value);
            else // switch
                Switch.PropertyInfo.SetValue(cmd, new SwitchParameter() { IsPresent = true });
        }

        private void ParseAndSetPropertyValue(InputCommand cmd, string value)
        {
            // get parser

            var parser = StringParsing.GetParser(Parameter.PropertyInfo.PropertyType);
            if(parser == null)
                throw new ArgumentException(string.Format("The parameter '{0}' has a property type of {1}. There are no available property input TypeParsers available for that type."
                        , Parameter.PropertyInfo.Name, Parameter.PropertyInfo.PropertyType));
            
            Parameter.PropertyInfo.SetValue(cmd, parser.Parse(value, Parameter.PropertyInfo.PropertyType));
        }
    }
}
