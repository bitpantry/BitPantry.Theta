using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Host.WindowsForms
{
    public class CopyEventArgs : EventArgs
    {
        public bool Handled { get; set; }
        public string Content { get; set; }
    }
}
