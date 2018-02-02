using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitPantry.Theta.Host.WindowsForms.InputEventsFilter
{
    class CommandExecutionPromptEventsFilter : IInputEventsFilter
    {
        private Action<KeyInputFilterResult> _handleResultAction = null;

        public bool IsEngaged { get; private set; }

        public CommandExecutionPromptEventsFilter(Action<KeyInputFilterResult> handleResultAction)
        {
            IsEngaged = true;
            _handleResultAction = handleResultAction;
        }

        public void HandleKeyDown(InputEventsFilterArgs args, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (args.InputPosition >= 0)
                        _handleResultAction(KeyInputFilterResult.Input_CommandExecutionSubmit);
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Up:
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Down:
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        public void HandleKeyUp(InputEventsFilterArgs args, System.Windows.Forms.KeyEventArgs e)
        {
            
        }

        public void HandleKeyPress(InputEventsFilterArgs args, System.Windows.Forms.KeyPressEventArgs e)
        {
            
        }

        public void HandleCopy(CopyEventArgs e)
        {

        }

        public void Engage() { IsEngaged = true; }
        public void Disengage() { IsEngaged = false; }
    }
}
