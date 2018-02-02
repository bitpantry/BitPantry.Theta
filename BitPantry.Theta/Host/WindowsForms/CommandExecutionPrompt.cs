using BitPantry.Theta.Component;
using BitPantry.Theta.Host.WindowsForms.InputEventsFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitPantry.Theta.Host.WindowsForms
{
    class CommandExecutionPrompt
    {
        private object _locker = new object();

        public bool IsActive { get; private set; }

        private HostInterface _interface = null;
        private string _inputString = null;

        public CommandExecutionPrompt(HostInterface hostInterface)
        {
            this._interface = hostInterface;
            this._interface.InputFilters.RegisterSubscription(this.HandleKeyInputResultAction);

            this.IsActive = false;
        }

        public string Prompt(string prompt)
        {
            this.ActivationGate();

            try
            {
                this._interface.WritePrompt(prompt);

                this._interface.InputFilters.SetFilterMode(FilterMode.CommandExecutionPrompt);

                lock (this._locker)
                    System.Threading.Monitor.Wait(this._locker);

                return this._inputString;
            }
            finally
            {
                this._interface.InputFilters.SetFilterMode(FilterMode.Execution);
                this.IsActive = false;
                this._inputString = null;
            }
        }

        private void HandleKeyInputResultAction(InputEventsFilterHandlerArgs args)
        {
            switch (args.Result)
            {
                case KeyInputFilterResult.Input_CommandExecutionSubmit:
                    this._inputString = this._interface.GetCurrentInputString();
                    this._interface.Out.Standard.WriteLine();
                    lock (this._locker)
                        System.Threading.Monitor.Pulse(this._locker);
                    break;
            }
        }

        private void ActivationGate()
        {
            lock (this._locker)
            {
                if (this.IsActive)
                    throw new InvalidOperationException("Another prompt operation is already active");

                this.IsActive = true;
            }
        }


    }
}
