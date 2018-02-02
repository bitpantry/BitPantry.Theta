using BitPantry.Theta.API;
using BitPantry.Theta.Host.WindowsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using BitPantry.Theta.Host;

namespace BitPantry.Theta.Utility.Console
{
    public partial class UtilityConsole : Form
    {
        public IHostInterface Host { get; private set; }

        private Type[] _modules = null;
        private Type[] _commands = null;

        public UtilityConsole(Type[] commands = null, Type[] modules = null, ICommandActivatorContainer container = null)
        {
            InitializeComponent();

            this._modules = modules ?? new Type[] { };
            this._commands = commands ?? new Type[] { };

            var host = new HostInterface(HostInterfaceMode.Output);

            if (container != null)
                host.SetCommandActivatorContainer(container);

            host.Dock = DockStyle.Fill;
            this.Controls.Add(host);

            this.Host = host;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (this.InstallTypes())
            {
                this.Host.Clear();
                this.Host.Mode = HostInterfaceMode.Interactive;
            }
            else
            {
                this.Host.Out.Error.WriteLine("The utility console has initialized with errors.");
                this.Host.Mode = HostInterfaceMode.Interactive;
            }

        }

        private bool InstallTypes()
        {
            foreach (var module in this._modules)
            {
                if (!this.Host.Modules.Install(module, this.Host.Out))
                    return false;
            }

            try
            {
                this.Host.Commands.Register(this._commands);                    
            }
            catch (Exception ex)
            {
                this.Host.Out.Error.WriteLine(string.Format("Commands could not be registered :: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace));
                return false;
            }

            return true;
        }

    }
}
