using Credera.Theta.Host;
using Credera.Theta.Host.WindowsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Credera.Theta
{
    public partial class frmShell : Form
    {
        public frmShell()
        {
            InitializeComponent();

            //HostRtb rtb = new HostRtb();
            //rtb.Dock = DockStyle.Fill;
            //this.Controls.Add(rtb);

            HostInterface i = new HostInterface();
            i.Dock = DockStyle.Fill;
            this.Controls.Add(i);
        }



    }
}
