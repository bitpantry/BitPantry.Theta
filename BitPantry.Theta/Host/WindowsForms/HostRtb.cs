using BitPantry.Theta.Host.WindowsForms.AutoComplete;
using BitPantry.Theta.Host.WindowsForms.InputEventsFilter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitPantry.Theta.Host.WindowsForms
{
    class HostRtb : RichTextBox
    {

        public HostRtb()
        {
            // initialize layout

            base.FontHeight = 10;
            base.BorderStyle = System.Windows.Forms.BorderStyle.None;
            base.Multiline = true;
            base.WordWrap = false;
            base.ScrollBars = RichTextBoxScrollBars.Both;
            base.AutoWordSelection = false;
        }

    }
}
