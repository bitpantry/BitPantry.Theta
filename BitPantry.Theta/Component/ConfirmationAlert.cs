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
            this._host = host;
        }

        public ConfirmationResult Confirm(string message, ConfirmationResult defaultResult)
        {
            var result = ConfirmationResult.Undefined;
            do
            {
                result = this.Confirm_INTERNAL(message, defaultResult);
                if (result == ConfirmationResult.Undefined)
                {
                    this._host.Out.Warning.WriteLine("Please enter [Y]es or [N]o.");
                    this._host.Out.Standard.WriteLine();
                }
            } while (result == ConfirmationResult.Undefined);
            return result;
       }

        private ConfirmationResult Confirm_INTERNAL(string message, ConfirmationResult defaultResult)
        {
            this._host.Out.Standard.WriteLine(message);
            this._host.Out.Standard.Write("Enter ");
            
            if(defaultResult == ConfirmationResult.Yes)
                this._host.Out.Accent2.Write("[Y]es");
            else
                this._host.Out.Standard.Write("[Y]es");

            this._host.Out.Standard.Write(" to confirm, or ");

            if(defaultResult == ConfirmationResult.No)
                this._host.Out.Accent2.Write("[N]o");
            else
                this._host.Out.Standard.Write("[N]o");

            this._host.Out.Standard.Write(" to cancel");

            if (defaultResult != ConfirmationResult.Undefined)
            {
                this._host.Out.Standard.Write(" (");

                if(defaultResult == ConfirmationResult.Yes)
                    this._host.Out.Standard.Write("Yes");
                else
                    this._host.Out.Standard.Write("No");

                this._host.Out.Standard.WriteLine(" is default).");
            }

            return this.EvaluateResponse(this._host.Prompt("Confirmation: "), defaultResult);
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
