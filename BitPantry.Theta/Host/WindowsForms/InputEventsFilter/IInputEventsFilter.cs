using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitPantry.Theta.Host.WindowsForms.InputEventsFilter
{
    interface IInputEventsFilter
    {
        bool IsEngaged { get; }

        void HandleKeyDown(InputEventsFilterArgs args, KeyEventArgs e);
        void HandleKeyUp(InputEventsFilterArgs args, KeyEventArgs e);
        void HandleKeyPress(InputEventsFilterArgs args, KeyPressEventArgs e);
        void HandleCopy(CopyEventArgs e);

        void Engage();
        void Disengage();
    }
}
