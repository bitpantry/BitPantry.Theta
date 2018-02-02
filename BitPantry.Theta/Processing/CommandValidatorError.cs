using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Processing
{
    enum CommandValidationType
    {
        MissingRequiredParameters,
        SuperflousParametersPresent,
        InvalidParameterValue,
        InvalidValidationValuesFunction
    }

    class CommandValidatorError
    {
        public CommandValidationType Type { get; set; }
        public List<ParameterDef> Parameters { get; set; }

        public string Message { get; set; }
        public Exception Error { get; set; }
    }
}
