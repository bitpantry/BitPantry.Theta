using BitPantry.Theta.Component;
using BitPantry.Theta.Host.WindowsForms.InputEventsFilter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitPantry.Theta.Host.WindowsForms.AutoComplete
{
    class AutoCompleteController
    {
        private HostInterface _hostInterface = null;
        private AutoCompleteProvider _provider = null;
        private frmAutoComplete _autoCompleteForm = null;

        private Input _currentInput = null;
        private InputNode _currentInputNode = null;

        public AutoCompleteController(HostInterface hostInterface)
        {
            _hostInterface = hostInterface;
            _hostInterface.RTB.LostFocus += _rtb_LostFocus;
            _hostInterface.RTB.TextChanged += _rtb_TextChanged;
            _hostInterface.RTB.SelectionChanged += _rtb_SelectionChanged;

            _provider = new AutoCompleteProvider(_hostInterface);

            _hostInterface.InputFilters.RegisterSubscription(HandleKeyInputResultAction);            
        }

        private void HandleKeyInputResultAction(InputEventsFilterHandlerArgs args)
        {
            switch (args.Result)
            {
                case KeyInputFilterResult.AutoComplete_Start:
                    if(_hostInterface.InputPosition > -1)
                        DoAutoComplete();
                    break;
                case KeyInputFilterResult.AutoComplete_Cancel:
                    CancelCurrentAutoComplete();
                    break;
                case KeyInputFilterResult.AutoComplete_SelectPrevious:
                    SelectPreviousOption();
                    break;
                case KeyInputFilterResult.AutoComplete_SelectNext:
                    SelectNextOption();
                    break;
                case KeyInputFilterResult.AutoComplete_Submit:
                    SubmitCurrentSelection();
                    break;
            }
        }

        void _rtb_LostFocus(object sender, EventArgs e)
        {
            CancelCurrentAutoComplete();
        }

        void _rtb_TextChanged(object sender, EventArgs e)
        {
            FilterOptionsByCurrentNode();
        }

        void _rtb_SelectionChanged(object sender, EventArgs e)
        {
            EvaluateNodeScope();
        }

        #region AUTO COMPLETE ACTIONS

        private void DoAutoComplete()
        {
            // create form if not exists

            if (_autoCompleteForm == null)
            {
                // initialize and wire up new form

                _autoCompleteForm = new frmAutoComplete();
                _autoCompleteForm.AutoCompleteComplete += _autoCompleteForm_AutoCompleteComplete;

                // install message filter to listen for lost focus

                AutoCompleteMessageFilter.Install(_autoCompleteForm);
            }

            // set auto complete options

            _currentInput = _hostInterface.GetCurrentInput();
            _currentInputNode = _currentInput.GetNodeAtPosition(_hostInterface.InputPosition);

            List<string> autoCompleteOptions = _provider.GetOptions(_currentInput, _currentInputNode);

            if (autoCompleteOptions != null && autoCompleteOptions.Count == 1)
            {
                string value = autoCompleteOptions[0];
                if (value.IndexOf(' ') > 0)
                    value = string.Format("\"{0}\"", value);
                _hostInterface.ReplaceCurrentNode(value);
            }
            else if (autoCompleteOptions != null && autoCompleteOptions.Count > 1) // only show if options are available
            {
                _autoCompleteForm.SetAutoCompleteOptions(autoCompleteOptions);

                // get the location of the caret, plus the line height as the auto complete form open point

                _autoCompleteForm.Location = GetPopupLocation(_hostInterface.CaretLocation, _hostInterface.RTB.SelectionFont.Height);

                // open the auto complete form

                _autoCompleteForm.Show();

                // adjust key input filter settings

                _hostInterface.InputFilters.SetFilterMode(FilterMode.AutoComplete);
            }
     
        }

        void _autoCompleteForm_AutoCompleteComplete(DialogResult result)
        {
            _hostInterface.InputFilters.SetFilterMode(FilterMode.Standard);
            if (result == DialogResult.OK)
            {
                string value = _autoCompleteForm.SelectedValue;
                if (value.IndexOf(' ') > 0)
                    value = string.Format("\"{0}\"", value);
                _hostInterface.ReplaceCurrentNode(value);
            }

            _currentInput = null;
            _currentInputNode = null;
        }

        private Point GetPopupLocation(Point targetLocation, int lineHeight)
        {
            int padding = 5;

            Point pt = new Point()
            {
                X = targetLocation.X,
                Y = targetLocation.Y
            };
            
            // adjusts for popup height

            if (Screen.PrimaryScreen.WorkingArea.Height < targetLocation.Y + _autoCompleteForm.OptionsBoxHeight)
                pt.Y = targetLocation.Y - _autoCompleteForm.OptionsBoxHeight - lineHeight - padding; // display popup above line
            else
                pt.Y += padding; // add padding to display below

            // adjusts for popup width

            if (Screen.PrimaryScreen.WorkingArea.Width < targetLocation.X + _autoCompleteForm.OptionsBoxWidth)
                pt.X = targetLocation.X - _autoCompleteForm.OptionsBoxWidth; // display popup to left

            return pt;
        }

        private void FilterOptionsByCurrentNode()
        {
            if (_autoCompleteForm != null && _autoCompleteForm.Visible)
            {
                InputNode currentNode = _hostInterface.GetCurrentInput().GetNodeAtPosition(_hostInterface.InputPosition);
                _autoCompleteForm.FilterAutoCompleteOptions(currentNode == null ? null : currentNode.Value);
            }
        }

        private void EvaluateNodeScope()
        {
            if (_autoCompleteForm != null && _autoCompleteForm.Visible) 
            {
                InputNode node = _hostInterface.GetCurrentInput().GetNodeAtPosition(_hostInterface.InputPosition);

                if (node == null) // started empty and is empty again
                    _autoCompleteForm.Cancel();
                else if (_currentInputNode == null && node != null && node.ElementType != InputNodeType.Command) // command - auto complete from empty input
                    _autoCompleteForm.Cancel();
                else if (node.ElementType != InputNodeType.Command && _currentInputNode.ElementType == InputNodeType.Empty
                    && node.Index == _currentInputNode.Index + 1) // ordinal parameter
                    return;
                else if (_currentInputNode != null && node.Index != _currentInputNode.Index) // indexes no longer match, moved to a different node
                    _autoCompleteForm.Cancel();
            }
        }

        private void CancelCurrentAutoComplete() { if (_autoCompleteForm != null) _autoCompleteForm.Cancel(); }

        private void SelectNextOption() { if (_autoCompleteForm != null) _autoCompleteForm.SelectNextOption(); }

        private void SelectPreviousOption() { if (_autoCompleteForm != null) _autoCompleteForm.SelectPreviousOption(); }

        private void SubmitCurrentSelection() { if (_autoCompleteForm != null) _autoCompleteForm.SubmitCurrentSelection(); }

        #endregion

    }
}
