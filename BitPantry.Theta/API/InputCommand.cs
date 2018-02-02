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
            Host = hostInterface;

            Out = new InputCommandOutputCollection(
                Host != null ? Host.Out.Standard : _nullWriter,
                Host != null ? Host.Out.Warning : _nullWriter,
                Host != null ? Host.Out.Error : _nullWriter,
                Debug.IsPresent && Host != null ? Host.Out.Debug : _nullWriter,
                Verbose.IsPresent && Host != null ? Host.Out.Verbose : _nullWriter,
                Host != null ? Host.Out.Accent1 : _nullWriter,
                Host != null ? Host.Out.Accent2 : _nullWriter,
                Host != null ? Host.Out.Accent3 : _nullWriter,
                Host != null ? Host.Out.Accent4 : _nullWriter,
                Host != null ? Host.Out.Accent5 : _nullWriter,
                Host != null ? Host.Out.Object : (IObjectWriter) new NullObjectWriter());
        }

        public abstract void Invoke(CommandInvocationContext context);

        public virtual List<string> GetDetailsDocumentation() { return null; }
        public virtual List<string> GetExamplesDocumentation() { return null; }
        
        public virtual void OnCancelExecutionRequest() { /* DO NOTHING */ }

        protected string Prompt(string prompt)
        {
            return Host.Prompt(prompt);
        }

        protected InputCommand()
        {
            _nullWriter = new NullWriter();
        }

        protected bool Confirm(string message, ConfirmationResult defaultResult = ConfirmationResult.Yes)
        {
            return new ConfirmationAlert(Host).Confirm(message, defaultResult) == ConfirmationResult.Yes ? true : false;
        }
    }
}
