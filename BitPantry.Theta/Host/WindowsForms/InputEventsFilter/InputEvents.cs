using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitPantry.Theta.Host.WindowsForms.InputEventsFilter
{
    class InputEvents
    {
        public KeyEventArgs KeyUpData { get; set; }
        public KeyEventArgs KeysDownData { get; set; }
        public KeyPressEventArgs KeyPressData { get; set; }
        public CopyEventArgs CopyEventArgs { get; set; }

        public InputEvents()
        {
            this.KeyUpData = null;
            this.KeysDownData = null;
            this.KeyPressData = null;
            this.CopyEventArgs = null;
        }
    }
}
