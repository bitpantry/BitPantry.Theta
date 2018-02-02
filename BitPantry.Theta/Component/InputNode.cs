using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component
{
    enum InputNodeType
    {
        Command,
        NamedParameterValue,
        NamedParameter,
        OrdinalParameterValue,
        Switch,
        Empty
    }

    class InputNode
    {
        private readonly string[] VALID_PREFIXES = new string[]
        {
            Constants.ELEMENT_PREFIX_NAMED_PARAMETER_SWITCH.ToString(),
            Constants.ELEMENT_PREFIX_BOOLEAN_SWITCH.ToString()
        };

        public string Element { get; private set; }
        public string Value { get { return Element.Trim().Trim('"').TrimStart(Constants.ELEMENT_PREFIXES); } }
        public InputNodeType ElementType { get; private set; }
        public int Index { get; set; }
        public int StartPosition { get; private set; }
        public int EndPosition { get; private set; }
        public InputNode IsPairedWith { get; set; }

        public InputNode(string element, int index, int locationStart, int locationEnd, InputNode previousNode)
        {
            Element = element;
            Index = index;
            StartPosition = locationStart;
            EndPosition = locationEnd;

            if (element.Trim().StartsWith(Constants.ELEMENT_PREFIX_NAMED_PARAMETER_SWITCH.ToString()))
            {
                ElementType = InputNodeType.NamedParameter;
            }
            else if (element.Trim().StartsWith(Constants.ELEMENT_PREFIX_BOOLEAN_SWITCH.ToString()))
            {
                ElementType = InputNodeType.Switch;
            }
            else // standard string input (concurrent or quoted), may be a parameter value or an ordinal parameter
            {
                if (!string.IsNullOrWhiteSpace(Element) 
                    && previousNode != null 
                    && previousNode.ElementType == InputNodeType.NamedParameter) // string value appearing right after named parameter switch
                {
                    ElementType = InputNodeType.NamedParameterValue;
                    IsPairedWith = previousNode;
                    previousNode.IsPairedWith = this;
                }
                else if (previousNode == null) // string value appearing as the first node
                {
                    ElementType = InputNodeType.Command;
                }
                else if (!string.IsNullOrWhiteSpace(Element))
                {
                    ElementType = InputNodeType.OrdinalParameterValue;
                }
                else // empty string
                {
                    ElementType = InputNodeType.Empty;
                }
            }
        }

        public override string ToString()
        {
            return Element;
        }

        

    }
}
