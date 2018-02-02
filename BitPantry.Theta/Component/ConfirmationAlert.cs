using BitPantry.Theta.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component
{
    public enum ConfirmationResult
    {
        Yes,
        No,
        Undefined
    }

    public class ConfirmationAlert
    {
        private const string MSG_DEFAULT = "Are you sure?";

        private IHostInterface _host = null;

        public ConfirmationAlert(IHostInterface host)
        {
            _host = host;
        }

        public ConfirmationResult Confirm(string message, ConfirmationResult defaultResult)
        {
            var result = ConfirmationResult.Undefined;
            do
            {
                result = Confirm_INTERNAL(message, defaultResult);
                if (result == ConfirmationResult.Undefined)
                {
                    _host.Out.Warning.WriteLine("Please enter [Y]es or [N]o.");
                    _host.Out.Standard.WriteLine();
                }
            } while (result == ConfirmationResult.Undefined);
            return result;
       }

        private ConfirmationResult Confirm_INTERNAL(string message, ConfirmationResult defaultResult)
        {
            _host.Out.Standard.WriteLine(message);
            _host.Out.Standard.Write("Enter ");
            
            if(defaultResult == ConfirmationResult.Yes)
                _host.Out.Accent2.Write("[Y]es");
            else
                _host.Out.Standard.Write("[Y]es");

            _host.Out.Standard.Write(" to confirm, or ");

            if(defaultResult == ConfirmationResult.No)
                _host.Out.Accent2.Write("[N]o");
            else
                _host.Out.Standard.Write("[N]o");

            _host.Out.Standard.Write(" to cancel");

            if (defaultResult != ConfirmationResult.Undefined)
            {
                _host.Out.Standard.Write(" (");

                if(defaultResult == ConfirmationResult.Yes)
                    _host.Out.Standard.Write("Yes");
                else
                    _host.Out.Standard.Write("No");

                _host.Out.Standard.WriteLine(" is default).");
            }

            return EvaluateResponse(_host.Prompt("Confirmation: "), defaultResult);
        }

        private ConfirmationResult EvaluateResponse(string response, ConfirmationResult defaultResult)
        {
            switch (response.Trim().ToUpper())
            {
                case "Y":
                case "YES":
                    return ConfirmationResult.Yes;
                case "N":
                case "NO":
                    return ConfirmationResult.No;
                case "":
                    return defaultResult;
                default:
                    return ConfirmationResult.Undefined;
            }
        }
    }
}
