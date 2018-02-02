using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BitPantry.Theta.Utility.Console.TestDi;
using BitPantry.Theta.Utility.Console.TestParsing;

namespace BitPantry.Theta.Utility.Console
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var console = new UtilityConsole(null, new Type[]
                {
                    typeof(Modules.Packages.Module),
                    typeof(Modules.Variables.Module),
                    typeof(TestDiModule),
                    typeof(TestParsingModule)
                }, new NinjectCommandActivator());

            Application.Run(console);
        }
    }
}
