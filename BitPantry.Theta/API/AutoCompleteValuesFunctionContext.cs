using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPantry.Theta.Component;

namespace BitPantry.Theta.API
{
    public class AutoCompleteValuesFunctionContext
    {
        public List<string> Values { get; set; }

        internal AutoCompleteValuesFunctionContext()
        {
            Values = new List<string>();
        }

    }
}
