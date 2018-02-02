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
    class CommandInvoker
    {
        public bool IsExecuting { get; set; }

        private object _locker = new object();

        private IHostInterface _interface = null;

        private InputCommand _currentCommand = null;

        public CommandInvoker(IHostInterface hostInterface)
        {
            this._interface = hostInterface;
            this.IsExecuting = false;
        }

        public void Invoke(InputCommand command, bool promptForMissingParameters, Action<CommandInvokerResponse> commandInvocationCompleteHandler)
        {
            lock (this._locker)
            {
                if (this.IsExecuting)
                    throw new InvalidOperationException("Command execution is already in progress");

                this.IsExecuting = true;

                this._currentCommand = command;

                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback((object state) =>
                {

                    // build result

                    CommandInvokerResponse result = new CommandInvokerResponse() { Command = command };

                    // prompt for missing parameters

                    if (promptForMissingParameters)
                        this.PromptForMissingParameters(command);

                    if (this.ValidateCommand(command))
                    {
                        try
                        {
                            command.Invoke(new CommandInvocationContext());
                        }
                        catch (Exception ex)
                        {
                            result.InvocationError = ex;
                            Util.WriteException(this._interface, ex);
                        }
                    }

                    commandInvocationCompleteHandler(result);

                    _currentCommand = null;
                    this.IsExecuting = false;

                }));

            }

        }

        private bool ValidateCommand(InputCommand command)
        {
            CommandValidator validator = new CommandValidator(command, this._interface);

            foreach (var error in validator.Errors)
            {
                this._interface.Out.Error.WriteLine(error.Message);
                if (error.Error != null)
                    Util.WriteException(this._interface, error.Error);
            }

            return validator.Errors.Count == 0; 
        }

        private void PromptForMissingParameters(InputCommand command)
        {
            foreach (var param in new CommandParameterAnalysis(command).MissingRequiredParameters)
            {
                string value = null;
                do
                {
                    value = this._interface.Prompt(string.Format("Enter a value for '{0}': ", param.Name));
                    if (!string.IsNullOrEmpty(value))
                        param.PropertyInfo.SetValue(command, value);
                    else
                        this._interface.Out.Warning.WriteLine("Value for '{0}' is required.", param.Name);

                } while (string.IsNullOrEmpty(value));
            }
        }

        public void CancelExecution()
        {
            this._currentCommand.OnCancelExecutionRequest();
        }
    }
}
