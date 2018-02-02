using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.API
{
    /// <summary>
    /// Defines the required members for registering / unregistering and initializing / uninitializing a group of commands and features
    /// for a shell plug in
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// The unique name of the module
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The command types that should be registered upon module installation and de-registered when the module is uninstalled
        /// </summary>
        List<Type> CommandTypes { get; }

        /// <summary>
        /// The list of module types that are required as loaded prerequisites for this module
        /// </summary>
        List<Type> Dependencies { get; }

        /// <summary>
        /// All installation logic should be done here
        /// </summary>
        void Install();

        /// <summary>
        /// All uninstallation logic should be done here
        /// </summary>
        void Uninstall();
    }
}
