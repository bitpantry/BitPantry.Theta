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

            _modules = modules ?? new Type[] { };
            _commands = commands ?? new Type[] { };

            var host = new HostInterface(HostInterfaceMode.Output);

            if (container != null)
                host.SetCommandActivatorContainer(container);

            host.Dock = DockStyle.Fill;
            Controls.Add(host);

            Host = host;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (InstallTypes())
            {
                Host.Clear();
                Host.Mode = HostInterfaceMode.Interactive;
            }
            else
            {
                Host.Out.Error.WriteLine("The utility console has initialized with errors.");
                Host.Mode = HostInterfaceMode.Interactive;
            }

        }

        private bool InstallTypes()
        {
            foreach (var module in _modules)
            {
                if (!Host.Modules.Install(module, Host.Out))
                    return false;
            }

            try
            {
                Host.Commands.Register(_commands);                    
            }
            catch (Exception ex)
            {
                Host.Out.Error.WriteLine(string.Format("Commands could not be registered :: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace));
                return false;
            }

            return true;
        }

    }
}
