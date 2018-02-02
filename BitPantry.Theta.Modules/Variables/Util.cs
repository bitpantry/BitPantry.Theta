using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Variables
{
    class Util
    {
        public static string FormatVariableValueForOneLine(string value)
        {
            string[] lines = value.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string str = lines[0];
            if (str.Length > 100)
                return string.Format("{0} ...{1}", str.Substring(0, 100), lines.Count() > 1 ? " ..." : string.Empty);
            return string.Format("{0}{1}", str, lines.Count() > 1 ? " ... ..." : string.Empty);
        }
    }
}
