using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component.Writers
{
    class TableCell
    {
        public const int TARGET_WIDTH = 25;

        public string Value { get; set; }
        
        public List<string> Lines 
        { 
            get 
            { 
                // break lines by line return

                var lines = this.Value == null 
                    ? new List<string>() 
                    : this.Value.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();

                // break up long lines

                var resizedLines = new List<string>();
                foreach (var line in lines)
                    resizedLines.AddRange(this.BreakLongLine(line));

                // remove last line if empty

                if (string.IsNullOrWhiteSpace(resizedLines.Last()))
                    resizedLines.Remove(resizedLines.Last());

                return resizedLines;
            } 
        }

        private IEnumerable<string> BreakLongLine(string line)
        {
            var lines = new List<string>();
            var pieces = line.Split(' ');
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pieces.Length; i++)
            {
                sb.Append(' ');

                if (sb.Length > TARGET_WIDTH)
                {
                    lines.Add(sb.ToString().Trim());
                    sb = new StringBuilder();
                }

                sb.Append(pieces[i]);
            }

            lines.Add(sb.ToString().Trim());

            return lines;

        }

        public int CellWidth 
        {
           get 
           {
               if (this.Lines.Count == 0)
                   return 0;
                return this.Lines.Select(l => l.Length).Max();
           } 
        }

        public TableCell()
        {
            this.Value = null;
        }
        
    }
}
