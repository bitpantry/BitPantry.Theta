using BitPantry.Theta.API;
using BitPantry.Theta.Component;
using BitPantry.Theta.Component.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component.Modules
{

    public class ModuleCollection : IReadOnlyList<IModule>
    {
        private CommandCollection _commands = null;

        #region READONLY LIST IMPLEMENTATION

        private List<IModule> _modules = null;

        public IModule this[int index]
        {
            get { return _modules[index]; }
        }

        public int Count
        {
            get { return _modules.Count; }
        }

        public IEnumerator<IModule> GetEnumerator()
        {
            return _modules.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _modules.GetEnumerator();
        }

        #endregion

        internal ModuleCollection(CommandCollection commands)
        {
            _modules = new List<IModule>();
            _commands = commands;
        }
        
        public bool Install(Type moduleType, IWriterCollection writers)
        {
            return new ModuleInstaller(_commands, _modules, writers).Install(moduleType);
        }

        public bool Uninstall(string moduleName, IWriterCollection writers)
        {
            return new ModuleUninstaller(_commands, _modules, writers)
                .Uninstall(this.FirstOrDefault(m => m.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase)).GetType());
        }

        public bool Uninstall(Type moduleType, IWriterCollection writers)
        {
            return new ModuleUninstaller(_commands, _modules, writers).Uninstall(moduleType);
        }


    }
}
