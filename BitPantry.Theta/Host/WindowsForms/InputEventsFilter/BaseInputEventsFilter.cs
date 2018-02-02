using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitPantry.Theta.Host.WindowsForms.InputEventsFilter
{
    class BaseInputEventsFilter : IInputEventsFilter
    {
        private Action<KeyInputFilterResult> _handleResultAction = null;

        public bool IsEngaged { get; private set; }

        public BaseInputEventsFilter(Action<KeyInputFilterResult> handleResultAction)
        {
            this.IsEngaged = true;
            this._handleResultAction = handleResultAction;
        }

        public void HandleKeyDown(InputEventsFilterArgs args, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    if (args.InputPosition < 0) 
                        e.SuppressKeyPress = true;
                    break;
                case Keys.Back:
                    if (args.InputPosition <= 0) 
                        e.SuppressKeyPress = true;
                    break;
                case Keys.Left:
                    if (args.InputPosition == 0) 
                        e.SuppressKeyPress = true;
                    break;
                case Keys.Home:
                    if (args.InputPosition > 0)
                    {
                        if (e.Modifiers == Keys.Shift)
                            this._handleResultAction(KeyInputFilterResult.Input_SelectToStart);
                        else
                            this._handleResultAction(KeyInputFilterResult.Input_MoveToStart);
                    }
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        public void HandleKeyUp(InputEventsFilterArgs args, KeyEventArgs e)
        {
            
        }

        public void HandleKeyPress(InputEventsFilterArgs args, KeyPressEventArgs e)
        {
            if (args.InputPosition < 0)
            {
                e.Handled = true;
                return;
            }
            
            if (!char.IsLetterOrDigit(e.KeyChar)
                && !char.IsSeparator(e.KeyChar)
                && !char.IsPunctuation(e.KeyChar)
                && !char.IsSymbol(e.KeyChar)
                && (int)e.KeyChar != 27) // escape 
            {
                // ignore all of this
                e.Handled = true;
            }

        }

        public void HandleCopy(CopyEventArgs e)
        {

        }

        public void Engage() { this.IsEngaged = true; }
        public void Disengage() { this.IsEngaged = false; }


    }
}
