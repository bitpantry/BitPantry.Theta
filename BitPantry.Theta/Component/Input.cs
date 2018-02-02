using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component
{
    class Input
    {
        public int LeadingWhiteSpaceCount { get; set; }
        public List<InputNode> InputNodes { get; set; }
        
        public Input(string input)
        {
            this.ParseInput(input);
        }
         
        private void ParseInput(string input)
        {
            // identify leading white space

            this.LeadingWhiteSpaceCount = input.Length - input.TrimStart().Length;

            // create input nodes
            
            this.InputNodes = new List<InputNode>();

            // create input nodes

            var splitInput = this.SplitInput(input);
            for (int i = 0; i < splitInput.Count(); i++)
            {
                // add up length of previous input

                int locationStart = this.LeadingWhiteSpaceCount
                    + this.InputNodes.Select(n => n.EndPosition + 1 - n.StartPosition).Sum() + 1;
                
                this.InputNodes.Add(new InputNode(splitInput[i]
                    , i
                    , locationStart
                    , locationStart + splitInput[i].Length - 1 //+ (i == splitInput.Count - 1 ? 1 : 0) // increment final location end
                    , this.InputNodes.LastOrDefault(n => n.ElementType != InputNodeType.Empty)));

            }

        }

        private List<string> SplitInput(string input)
        {
            char delimiter = ' ';

            Regex csvPreservingQuotedStrings = new Regex(string.Format("(\"[^\"]*\"|[^{0}])+|(\\s?)+", delimiter));
            var values =
                 csvPreservingQuotedStrings.Matches(input)
                .Cast<Match>()
                .Where(m => !string.IsNullOrEmpty(m.Value))
                .Select(m => m.Value);
            return values.ToList();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(string.Empty.PadLeft(this.LeadingWhiteSpaceCount, ' '));
            foreach (var node in this.InputNodes)
                sb.AppendFormat("{0}", node.ToString());
            return sb.ToString();
        }

        public InputNode GetNodeAtPosition(int position)
        {
            return this.InputNodes.Where(n => n.StartPosition <= position && n.EndPosition >= position).FirstOrDefault();
        }

        public InputNode GetCommandNode()
        {
            return this.InputNodes.FirstOrDefault(n => n.ElementType == InputNodeType.Command);
        }

    }
}
