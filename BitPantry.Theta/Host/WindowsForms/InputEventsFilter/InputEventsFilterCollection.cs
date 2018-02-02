using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitPantry.Theta.Host.WindowsForms.InputEventsFilter
{
    [Flags]
    enum KeyInputFilterResult : int
    {
        Undefined = 0,
        AutoComplete_Start = 2,
        AutoComplete_Cancel = 4,
        AutoComplete_SelectPrevious = 8,
        AutoComplete_SelectNext = 16,
        AutoComplete_Submit = 32,
        Input_Submit = 64,
        Input_ShowPreviousHistory = 128,
        Input_ShowNextHistory = 256,
        Input_Focus = 1024,
        Input_MoveToStart = 2048,
        Input_SelectToStart = 4096,
        Input_CommandExecutionSubmit = 8192,
        Exec_CancelCommandExecution = 16384
    }

    public enum FilterMode
    {
        Execution, // previously FullBlock
        Standard,
        CommandExecutionPrompt,
        AutoComplete
    }

    class InputEventsFilterCollection
    {
        private enum FilterKey
        {
            FullBlock,
            Base,
            Standard,
            CommandExecutionPrompt,
            AutoComplete
        }

        private HostInterface _hostInterface = null;

        private List<Action<InputEventsFilterHandlerArgs>> _subscriptions = null;

        private Dictionary<FilterKey, IInputEventsFilter> _filters = null;

        public InputEvents EventArgs { get; set; }

        public InputEventsFilterCollection(HostInterface hostInterface) 
        {
            this._hostInterface = hostInterface;

            // initialize filters

            this._filters = new Dictionary<FilterKey, IInputEventsFilter>()
            {
                { FilterKey.FullBlock, new ExecutionBlockEventsFilter(this.HandleResultAction) },
                { FilterKey.Base, new BaseInputEventsFilter(this.HandleResultAction) },
                { FilterKey.Standard, new StandardInputEventsFilter(this.HandleResultAction) },
                { FilterKey.CommandExecutionPrompt, new CommandExecutionPromptEventsFilter(this.HandleResultAction) },
                { FilterKey.AutoComplete, new AutoCompleteInputEventsFilter(this.HandleResultAction) }
            };

            // initialize subscriptions

            this._subscriptions = new List<Action<InputEventsFilterHandlerArgs>>();

            // wire up events

            this._hostInterface.RTB.KeyDown += rtb_KeyDown;
            this._hostInterface.RTB.KeyUp += rtb_KeyUp;
            this._hostInterface.RTB.KeyPress += rtb_KeyPress;
            this._hostInterface.OnCopy += _hostInterface_OnCopy;
        }

        private void HandleResultAction(KeyInputFilterResult result)
        {
            var args = new InputEventsFilterHandlerArgs(result);
            foreach (var subscription in this._subscriptions)
            {
                subscription(args);
                if (args.IsHandled)
                    break;
            }
        }

        public void RegisterSubscription(Action<InputEventsFilterHandlerArgs> resultHandlerAction)
        {
            this._subscriptions.Add(resultHandlerAction);
        }

        public void SetFilterMode(FilterMode mode)
        {
            switch (mode)
            {
                case FilterMode.Execution:
                    this._filters[FilterKey.FullBlock].Engage();
                    this._filters[FilterKey.Base].Disengage();
                    this._filters[FilterKey.AutoComplete].Disengage();
                    this._filters[FilterKey.CommandExecutionPrompt].Disengage();
                    this._filters[FilterKey.Standard].Disengage();
                    break;
                case FilterMode.Standard:
                    this._filters[FilterKey.FullBlock].Disengage();
                    this._filters[FilterKey.Base].Engage();
                    this._filters[FilterKey.AutoComplete].Disengage();
                    this._filters[FilterKey.CommandExecutionPrompt].Disengage();
                    this._filters[FilterKey.Standard].Engage();
                    break;
                case FilterMode.CommandExecutionPrompt:
                    this._filters[FilterKey.FullBlock].Disengage();
                    this._filters[FilterKey.Base].Engage();
                    this._filters[FilterKey.AutoComplete].Disengage();
                    this._filters[FilterKey.CommandExecutionPrompt].Engage();
                    this._filters[FilterKey.Standard].Disengage();
                    break;
                case FilterMode.AutoComplete:
                    this._filters[FilterKey.FullBlock].Disengage();
                    this._filters[FilterKey.Base].Engage();
                    this._filters[FilterKey.AutoComplete].Engage();
                    this._filters[FilterKey.CommandExecutionPrompt].Disengage();
                    this._filters[FilterKey.Standard].Disengage();
                    break;
            }
        }

        #region FILTER HANDLERS

        private InputEventsFilterArgs CreateKeyInputFilterArgs()
        {
            return new InputEventsFilterArgs() { InputPosition = this._hostInterface.InputPosition };
        }

        void rtb_KeyPress(object sender, KeyPressEventArgs e) { this.HandleKeyPress(this.CreateKeyInputFilterArgs(), e); }
        void rtb_KeyUp(object sender, KeyEventArgs e) { this.HandleKeyUp(this.CreateKeyInputFilterArgs(), e); }
        void rtb_KeyDown(object sender, KeyEventArgs e) { this.HandleKeyDown(this.CreateKeyInputFilterArgs(), e); }

        private void HandleKeyDown(InputEventsFilterArgs args, KeyEventArgs e)
        {
            this.EventArgs = new InputEvents() { KeysDownData = e };

            foreach (var filter in this._filters.Values)
            {
                if (filter.IsEngaged)
                {
                    filter.HandleKeyDown(args, e);
                    if (e.SuppressKeyPress) break;
                }
            }
        }

        private void HandleKeyUp(InputEventsFilterArgs args, KeyEventArgs e)
        {
            this.EventArgs = new InputEvents() { KeyUpData = e };

            foreach (var filter in this._filters.Values)
            {
                if (filter.IsEngaged)
                {
                    filter.HandleKeyUp(args, e);
                    if (e.SuppressKeyPress) break;
                }
            }
        }

        private void HandleKeyPress(InputEventsFilterArgs args, KeyPressEventArgs e)
        {
            this.EventArgs = new InputEvents() { KeyPressData = e };

            foreach (var filter in this._filters.Values)
            {
                if (filter.IsEngaged)
                {
                    filter.HandleKeyPress(args, e);
                    if (e.Handled) break;
                }
            }
        }

        private void _hostInterface_OnCopy(CopyEventArgs e)
        {
            this.EventArgs = new InputEvents() { CopyEventArgs = e };

            foreach (var filter in this._filters.Values)
            {
                if (filter.IsEngaged)
                {
                    filter.HandleCopy(e);
                    if (e.Handled) break;                        
                }
            }
        }

        #endregion

    }
}
