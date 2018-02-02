using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitPantry.Theta.Host.WindowsForms.InputEventsFilter
{
    class StandardInputEventsFilter : IInputEventsFilter
    {
        private Action<KeyInputFilterResult> _handleResultAction = null;

        public bool IsEngaged { get; private set; }

        public StandardInputEventsFilter(Action<KeyInputFilterResult> handleResultAction)
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
                        _handleResultAction(KeyInputFilterResult.Input_Submit);
                        e.SuppressKeyPress = true;
                        break;
                 case Keys.Up:
                        if (args.InputPosition < 0)
                        _handleResultAction(KeyInputFilterResult.Input_Focus);
                    _handleResultAction(KeyInputFilterResult.Input_ShowPreviousHistory);
                        e.SuppressKeyPress = true;
                        break;
                 case Keys.Down:
                        if (args.InputPosition < 0)
                        _handleResultAction(KeyInputFilterResult.Input_Focus);
                    _handleResultAction(KeyInputFilterResult.Input_ShowNextHistory);
                        e.SuppressKeyPress = true;
                        break;
            }
        }

        public void HandleKeyUp(InputEventsFilterArgs args, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Tab:
                    _handleResultAction(KeyInputFilterResult.AutoComplete_Start);
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        public void HandleKeyPress(InputEventsFilterArgs args, KeyPressEventArgs e)
        {

        }

        public void HandleCopy(CopyEventArgs e)
        {

        }

        public void Engage() { IsEngaged = true; }
        public void Disengage() { IsEngaged = false; }
    }
}
