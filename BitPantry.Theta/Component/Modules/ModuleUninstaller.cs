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
    class ModuleUninstaller
    {
        private CommandCollection _commands = null;
        private List<IModule> _modules = null;
        private IWriterCollection _writers = null;

        public ModuleUninstaller(CommandCollection commands, List<IModule> modules, IWriterCollection writers)
        {
            _commands = commands;
            _modules = modules;
            _writers = writers;
        }

        public bool Uninstall(Type moduleType)
        {
            IModule module = null;
            
            try
            {
                module = (IModule)Activator.CreateInstance(moduleType);

                _writers.Verbose.WriteLine(string.Format("Uninstalling module '{0}' ...", module.Name));

                if (!IsDependency(moduleType))
                {
                    // remove self 

                    if (module.CommandTypes != null)
                        _commands.Unregister(module.CommandTypes.ToArray());

                    module.Uninstall();

                    _modules.Remove(_modules.FirstOrDefault(m => m.GetType() == moduleType));

                    // remove dependencies

                    if (module.Dependencies != null)
                    {
                        foreach (var dependencyType in module.Dependencies)
                        {
                            if (!IsDependency(dependencyType))
                                Uninstall(dependencyType);
                            else
                                _writers.Verbose.WriteLine(string.Format("Dependency '{0}' is a dependency for other installed modules and will not be uninstalled.", dependencyType));
                        }
                    }
                }
                else
                {
                    _writers.Warning.WriteLine(string.Format("Module '{0}' is a dependency and cannot be uninstalled. Uninstall modules that are dependent first.", module.Name));
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _writers.Error.WriteLine(string.Format("Module '{0}' failed to uninstall :: {1}\r\n{2}"
                    , module == null ? moduleType.ToString() : module.Name, ex.Message, ex.StackTrace));
                return false;
            }
        }

        private bool IsDependency(Type moduleType)
        {
            foreach (var installedModule in _modules)
            {
                if (installedModule.Dependencies != null && installedModule.Dependencies.Contains(moduleType))
                    return true;
            }

            return false;
        }
    }
}
