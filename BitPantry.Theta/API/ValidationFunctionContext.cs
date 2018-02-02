using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.API
{
    public class ValidationFunctionContext
    {
        public string Value { get; set; }

        public bool IsValid { get; set; }
        public string Message { get; set; }
 
        internal ValidationFunctionContext(string value)
        {
            this.Value = value;

            this.IsValid = true;
        }
    }
}
