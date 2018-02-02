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
            _hostInterface = hostInterface;

            // initialize filters

            _filters = new Dictionary<FilterKey, IInputEventsFilter>()
            {
                { FilterKey.FullBlock, new ExecutionBlockEventsFilter(HandleResultAction) },
                { FilterKey.Base, new BaseInputEventsFilter(HandleResultAction) },
                { FilterKey.Standard, new StandardInputEventsFilter(HandleResultAction) },
                { FilterKey.CommandExecutionPrompt, new CommandExecutionPromptEventsFilter(HandleResultAction) },
                { FilterKey.AutoComplete, new AutoCompleteInputEventsFilter(HandleResultAction) }
            };

            // initialize subscriptions

            _subscriptions = new List<Action<InputEventsFilterHandlerArgs>>();

            // wire up events

            _hostInterface.RTB.KeyDown += rtb_KeyDown;
            _hostInterface.RTB.KeyUp += rtb_KeyUp;
            _hostInterface.RTB.KeyPress += rtb_KeyPress;
            _hostInterface.OnCopy += _hostInterface_OnCopy;
        }

        private void HandleResultAction(KeyInputFilterResult result)
        {
            var args = new InputEventsFilterHandlerArgs(result);
            foreach (var subscription in _subscriptions)
            {
                subscription(args);
                if (args.IsHandled)
                    break;
            }
        }

        public void RegisterSubscription(Action<InputEventsFilterHandlerArgs> resultHandlerAction)
        {
            _subscriptions.Add(resultHandlerAction);
        }

        public void SetFilterMode(FilterMode mode)
        {
            switch (mode)
            {
                case FilterMode.Execution:
                    _filters[FilterKey.FullBlock].Engage();
                    _filters[FilterKey.Base].Disengage();
                    _filters[FilterKey.AutoComplete].Disengage();
                    _filters[FilterKey.CommandExecutionPrompt].Disengage();
                    _filters[FilterKey.Standard].Disengage();
                    break;
                case FilterMode.Standard:
                    _filters[FilterKey.FullBlock].Disengage();
                    _filters[FilterKey.Base].Engage();
                    _filters[FilterKey.AutoComplete].Disengage();
                    _filters[FilterKey.CommandExecutionPrompt].Disengage();
                    _filters[FilterKey.Standard].Engage();
                    break;
                case FilterMode.CommandExecutionPrompt:
                    _filters[FilterKey.FullBlock].Disengage();
                    _filters[FilterKey.Base].Engage();
                    _filters[FilterKey.AutoComplete].Disengage();
                    _filters[FilterKey.CommandExecutionPrompt].Engage();
                    _filters[FilterKey.Standard].Disengage();
                    break;
                case FilterMode.AutoComplete:
                    _filters[FilterKey.FullBlock].Disengage();
                    _filters[FilterKey.Base].Engage();
                    _filters[FilterKey.AutoComplete].Engage();
                    _filters[FilterKey.CommandExecutionPrompt].Disengage();
                    _filters[FilterKey.Standard].Disengage();
                    break;
            }
        }

        #region FILTER HANDLERS

        private InputEventsFilterArgs CreateKeyInputFilterArgs()
        {
            return new InputEventsFilterArgs() { InputPosition = _hostInterface.InputPosition };
        }

        void rtb_KeyPress(object sender, KeyPressEventArgs e) { HandleKeyPress(CreateKeyInputFilterArgs(), e); }
        void rtb_KeyUp(object sender, KeyEventArgs e) { HandleKeyUp(CreateKeyInputFilterArgs(), e); }
        void rtb_KeyDown(object sender, KeyEventArgs e) { HandleKeyDown(CreateKeyInputFilterArgs(), e); }

        private void HandleKeyDown(InputEventsFilterArgs args, KeyEventArgs e)
        {
            EventArgs = new InputEvents() { KeysDownData = e };

            foreach (var filter in _filters.Values)
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
            EventArgs = new InputEvents() { KeyUpData = e };

            foreach (var filter in _filters.Values)
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
            EventArgs = new InputEvents() { KeyPressData = e };

            foreach (var filter in _filters.Values)
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
            EventArgs = new InputEvents() { CopyEventArgs = e };

            foreach (var filter in _filters.Values)
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
