using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitPantry.Theta.Host.WindowsForms.InputEventsFilter
{
    class AutoCompleteInputEventsFilter : IInputEventsFilter
    {
        private Action<KeyInputFilterResult> _handleResultAction = null;

        public bool IsEngaged { get; private set; }

        public AutoCompleteInputEventsFilter(Action<KeyInputFilterResult> handleResultAction)
        {
            this.IsEngaged = true;
            this._handleResultAction = handleResultAction;
        }

        public void HandleKeyDown(InputEventsFilterArgs args, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    this._handleResultAction(KeyInputFilterResult.AutoComplete_Submit);
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Up:
                    this._handleResultAction(KeyInputFilterResult.Input_Focus);
                    this._handleResultAction(KeyInputFilterResult.AutoComplete_SelectPrevious);
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Down:
                    this._handleResultAction(KeyInputFilterResult.Input_Focus);
                    this._handleResultAction(KeyInputFilterResult.AutoComplete_SelectNext);
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Cancel:
                    Console.Write("cancel");
                    break;
            }
        }

        public void HandleKeyUp(InputEventsFilterArgs args, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Tab:
                    this._handleResultAction(KeyInputFilterResult.AutoComplete_Submit);
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        public void HandleKeyPress(InputEventsFilterArgs args, KeyPressEventArgs e)
        {
            switch ((int)e.KeyChar)
            {
                case 32: // space
                    this._handleResultAction(KeyInputFilterResult.AutoComplete_Submit);
                    e.Handled = true;
                    break;
                case 27: // escape
                    this._handleResultAction(KeyInputFilterResult.AutoComplete_Cancel);
                    e.Handled = true;
                    break;
            }
        }

        public void HandleCopy(CopyEventArgs e)
        {

        }

        public void Engage() { this.IsEngaged = true; }
        public void Disengage() { this.IsEngaged = false; }
    }
}
