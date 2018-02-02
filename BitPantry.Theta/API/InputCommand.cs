using BitPantry.Theta.Component;
using BitPantry.Theta.Component.Writers;
using BitPantry.Theta.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.API
{
    public abstract class InputCommand
    {
        private InterceptWriter _nullWriter = null;

        protected IHostInterface Host { get; private set; }

        public string ActiveParameterSet { get; internal set; }
       
        [Switch]
        [Synopsis("If present, output sent to the Verbose HostWriter will be written to the console.")]
        public SwitchParameter Verbose { get; set; }

        [Switch]
        [Synopsis("If present, output sent to the Debug HostWriter will be written to the console.")]
        public SwitchParameter Debug { get; set; }

        protected InputCommandOutputCollection Out { get; private set; }

        internal void InstallInterfaceServices(IHostInterface hostInterface)
        {
            this.Host = hostInterface;

            this.Out = new InputCommandOutputCollection(
                this.Host != null ? this.Host.Out.Standard : this._nullWriter,
                this.Host != null ? this.Host.Out.Warning : this._nullWriter,
                this.Host != null ? this.Host.Out.Error : this._nullWriter,
                this.Debug.IsPresent && this.Host != null ? this.Host.Out.Debug : this._nullWriter,
                this.Verbose.IsPresent && this.Host != null ? this.Host.Out.Verbose : this._nullWriter,
                this.Host != null ? this.Host.Out.Accent1 : this._nullWriter,
                this.Host != null ? this.Host.Out.Accent2 : this._nullWriter,
                this.Host != null ? this.Host.Out.Accent3 : this._nullWriter,
                this.Host != null ? this.Host.Out.Accent4 : this._nullWriter,
                this.Host != null ? this.Host.Out.Accent5 : this._nullWriter,
                this.Host != null ? this.Host.Out.Object : (IObjectWriter) new NullObjectWriter());
        }

        public abstract void Invoke(CommandInvocationContext context);

        public virtual List<string> GetDetailsDocumentation() { return null; }
        public virtual List<string> GetExamplesDocumentation() { return null; }
        
        public virtual void OnCancelExecutionRequest() { /* DO NOTHING */ }

        protected string Prompt(string prompt)
        {
            return this.Host.Prompt(prompt);
        }

        protected InputCommand()
        {
            this._nullWriter = new NullWriter();
        }

        protected bool Confirm(string message, ConfirmationResult defaultResult = ConfirmationResult.Yes)
        {
            return new ConfirmationAlert(this.Host).Confirm(message, defaultResult) == ConfirmationResult.Yes ? true : false;
        }
    }
}
