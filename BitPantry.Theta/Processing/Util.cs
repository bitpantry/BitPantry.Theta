using BitPantry.Theta.API;
using BitPantry.Theta.Component;
using BitPantry.Theta.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Processing
{
    static class Util
    {
        public static InputCommandDef GetCommand(CommandCollection commands, Input input)
        {
            InputNode commandNode = input.InputNodes.FirstOrDefault(c => c.ElementType == InputNodeType.Command);
            if (commandNode == null)
                return null;

            return commands.FirstOrDefault(c => c.CommandName.Equals(commandNode.Value, StringComparison.OrdinalIgnoreCase)
                || c.Aliases.Contains(commandNode.Value, StringComparer.OrdinalIgnoreCase));
        }

        public static void WriteException(IHostInterface hostInterface, Exception exception)
        {
            hostInterface.Out.Error.WriteLine(exception.Message);
            hostInterface.Out.Error.WriteLine(exception.StackTrace);
        }

        public static List<string> GetParameterAutoCompleteValues(CommandResolver resolver, ParameterDef paramDef, IHostInterface host)
        {
            if (string.IsNullOrWhiteSpace(paramDef.AutoCompleteValuesFunction))
                throw new ArgumentException(string.Format("Parameter definition for parameter {0} does not define an auto complete values function"
                    , paramDef.Name));

            var cmd = new CommandActivator().Create(resolver, host);

            var methodInfo = cmd.GetType().GetMethod(paramDef.AutoCompleteValuesFunction);
            if (methodInfo == null)
                throw new ArgumentException(string.Format("Auto complete values function '{0}' cannot be found for parameter {0}"
                    , paramDef.AutoCompleteValuesFunction, paramDef.Name));

            var context = new AutoCompleteValuesFunctionContext();
            methodInfo.Invoke(cmd, new object[] { context });

            return context.Values;      
        }

    }
}
