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
            this._hostInterface = hostInterface;
            this._hostInterface.RTB.LostFocus += _rtb_LostFocus;
            this._hostInterface.RTB.TextChanged += _rtb_TextChanged;
            this._hostInterface.RTB.SelectionChanged += _rtb_SelectionChanged;
            
            this._provider = new AutoCompleteProvider(this._hostInterface);

            this._hostInterface.InputFilters.RegisterSubscription(this.HandleKeyInputResultAction);            
        }

        private void HandleKeyInputResultAction(InputEventsFilterHandlerArgs args)
        {
            switch (args.Result)
            {
                case KeyInputFilterResult.AutoComplete_Start:
                    if(this._hostInterface.InputPosition > -1)
                        this.DoAutoComplete();
                    break;
                case KeyInputFilterResult.AutoComplete_Cancel:
                    this.CancelCurrentAutoComplete();
                    break;
                case KeyInputFilterResult.AutoComplete_SelectPrevious:
                    this.SelectPreviousOption();
                    break;
                case KeyInputFilterResult.AutoComplete_SelectNext:
                    this.SelectNextOption();
                    break;
                case KeyInputFilterResult.AutoComplete_Submit:
                    this.SubmitCurrentSelection();
                    break;
            }
        }

        void _rtb_LostFocus(object sender, EventArgs e)
        {
            this.CancelCurrentAutoComplete();
        }

        void _rtb_TextChanged(object sender, EventArgs e)
        {
            this.FilterOptionsByCurrentNode();
        }

        void _rtb_SelectionChanged(object sender, EventArgs e)
        {
            this.EvaluateNodeScope();
        }

        #region AUTO COMPLETE ACTIONS

        private void DoAutoComplete()
        {
            // create form if not exists

            if (this._autoCompleteForm == null)
            {
                // initialize and wire up new form

                this._autoCompleteForm = new frmAutoComplete();
                this._autoCompleteForm.AutoCompleteComplete += _autoCompleteForm_AutoCompleteComplete;

                // install message filter to listen for lost focus

                AutoCompleteMessageFilter.Install(this._autoCompleteForm);
            }

            // set auto complete options

            this._currentInput = this._hostInterface.GetCurrentInput();
            this._currentInputNode = this._currentInput.GetNodeAtPosition(this._hostInterface.InputPosition);

            List<string> autoCompleteOptions = this._provider.GetOptions(this._currentInput, this._currentInputNode);

            if (autoCompleteOptions != null && autoCompleteOptions.Count == 1)
            {
                string value = autoCompleteOptions[0];
                if (value.IndexOf(' ') > 0)
                    value = string.Format("\"{0}\"", value);
                this._hostInterface.ReplaceCurrentNode(value);
            }
            else if (autoCompleteOptions != null && autoCompleteOptions.Count > 1) // only show if options are available
            {
                this._autoCompleteForm.SetAutoCompleteOptions(autoCompleteOptions);

                // get the location of the caret, plus the line height as the auto complete form open point

                this._autoCompleteForm.Location = this.GetPopupLocation(this._hostInterface.CaretLocation, this._hostInterface.RTB.SelectionFont.Height);

                // open the auto complete form

                this._autoCompleteForm.Show();

                // adjust key input filter settings

                this._hostInterface.InputFilters.SetFilterMode(FilterMode.AutoComplete);
            }
     
        }

        void _autoCompleteForm_AutoCompleteComplete(DialogResult result)
        {
            this._hostInterface.InputFilters.SetFilterMode(FilterMode.Standard);
            if (result == DialogResult.OK)
            {
                string value = this._autoCompleteForm.SelectedValue;
                if (value.IndexOf(' ') > 0)
                    value = string.Format("\"{0}\"", value);
                this._hostInterface.ReplaceCurrentNode(value);
            }

            this._currentInput = null;
            this._currentInputNode = null;
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

            if (Screen.PrimaryScreen.WorkingArea.Height < targetLocation.Y + this._autoCompleteForm.OptionsBoxHeight)
                pt.Y = targetLocation.Y - this._autoCompleteForm.OptionsBoxHeight - lineHeight - padding; // display popup above line
            else
                pt.Y += padding; // add padding to display below

            // adjusts for popup width

            if (Screen.PrimaryScreen.WorkingArea.Width < targetLocation.X + this._autoCompleteForm.OptionsBoxWidth)
                pt.X = targetLocation.X - this._autoCompleteForm.OptionsBoxWidth; // display popup to left

            return pt;
        }

        private void FilterOptionsByCurrentNode()
        {
            if (this._autoCompleteForm != null && this._autoCompleteForm.Visible)
            {
                InputNode currentNode = this._hostInterface.GetCurrentInput().GetNodeAtPosition(this._hostInterface.InputPosition);
                this._autoCompleteForm.FilterAutoCompleteOptions(currentNode == null ? null : currentNode.Value);
            }
        }

        private void EvaluateNodeScope()
        {
            if (this._autoCompleteForm != null && this._autoCompleteForm.Visible) 
            {
                InputNode node = this._hostInterface.GetCurrentInput().GetNodeAtPosition(this._hostInterface.InputPosition);

                if (node == null) // started empty and is empty again
                    this._autoCompleteForm.Cancel();
                else if (this._currentInputNode == null && node != null && node.ElementType != InputNodeType.Command) // command - auto complete from empty input
                    this._autoCompleteForm.Cancel();
                else if (node.ElementType != InputNodeType.Command && this._currentInputNode.ElementType == InputNodeType.Empty
                    && node.Index == this._currentInputNode.Index + 1) // ordinal parameter
                    return;
                else if (this._currentInputNode != null && node.Index != this._currentInputNode.Index) // indexes no longer match, moved to a different node
                    this._autoCompleteForm.Cancel();
            }
        }

        private void CancelCurrentAutoComplete() { if (this._autoCompleteForm != null) this._autoCompleteForm.Cancel(); }

        private void SelectNextOption() { if (this._autoCompleteForm != null) this._autoCompleteForm.SelectNextOption(); }

        private void SelectPreviousOption() { if (this._autoCompleteForm != null) this._autoCompleteForm.SelectPreviousOption(); }

        private void SubmitCurrentSelection() { if (this._autoCompleteForm != null) this._autoCompleteForm.SubmitCurrentSelection(); }

        #endregion

    }
}
