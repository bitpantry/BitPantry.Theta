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
            _interface = hostInterface;
            _interface.InputFilters.RegisterSubscription(HandleKeyInputResultAction);

            IsActive = false;
        }

        public string Prompt(string prompt)
        {
            ActivationGate();

            try
            {
                _interface.WritePrompt(prompt);

                _interface.InputFilters.SetFilterMode(FilterMode.CommandExecutionPrompt);

                lock (_locker)
                    System.Threading.Monitor.Wait(_locker);

                return _inputString;
            }
            finally
            {
                _interface.InputFilters.SetFilterMode(FilterMode.Execution);
                IsActive = false;
                _inputString = null;
            }
        }

        private void HandleKeyInputResultAction(InputEventsFilterHandlerArgs args)
        {
            switch (args.Result)
            {
                case KeyInputFilterResult.Input_CommandExecutionSubmit:
                    _inputString = _interface.GetCurrentInputString();
                    _interface.Out.Standard.WriteLine();
                    lock (_locker)
                        System.Threading.Monitor.Pulse(_locker);
                    break;
            }
        }

        private void ActivationGate()
        {
            lock (_locker)
            {
                if (IsActive)
                    throw new InvalidOperationException("Another prompt operation is already active");

                IsActive = true;
            }
        }


    }
}
