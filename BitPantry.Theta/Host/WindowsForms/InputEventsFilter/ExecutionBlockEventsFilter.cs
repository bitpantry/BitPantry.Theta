using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitPantry.Theta.Host.WindowsForms.InputEventsFilter
{
    class ExecutionBlockEventsFilter : IInputEventsFilter
    {
        private readonly Action<KeyInputFilterResult> _handleResultAction;

        public bool IsEngaged { get; private set; }

        public ExecutionBlockEventsFilter(Action<KeyInputFilterResult> handleResultAction)
        {
            _handleResultAction = handleResultAction;
        }

        public void HandleKeyDown(InputEventsFilterArgs args, System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.C:
                    if (e.Modifiers == Keys.Control)
                        _handleResultAction(KeyInputFilterResult.Exec_CancelCommandExecution);
                    e.SuppressKeyPress = true;
                    break;
            }

            e.SuppressKeyPress = true;
        }

        public void HandleKeyUp(InputEventsFilterArgs args, System.Windows.Forms.KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        public void HandleKeyPress(InputEventsFilterArgs args, System.Windows.Forms.KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        public void HandleCopy(CopyEventArgs e)
        {
            e.Handled = true;
        }

        public void Engage() { IsEngaged = true; }
        public void Disengage() { IsEngaged = false; }
    }
}
