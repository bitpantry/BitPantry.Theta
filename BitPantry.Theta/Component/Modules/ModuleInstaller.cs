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
    class ModuleInstaller
    {
        private CommandCollection _commands = null;
        private List<IModule> _modules = null;
        private IWriterCollection _writers = null;

        public ModuleInstaller(CommandCollection commands, List<IModule> modules, IWriterCollection writers)
        {
            _commands = commands;
            _modules = modules;
            _writers = writers;
        }

        public bool Install(Type moduleType)
        {
            var module = (IModule)Activator.CreateInstance(moduleType);

            _writers.Verbose.WriteLine(string.Format("Installing module type '{0}' from assembly '{1}' ..."
                , moduleType, moduleType.Assembly));

            var status = Install_INTERNAL(moduleType);

            if (status > 0)
            {
                _writers.Error.WriteLine(string.Format("The installation of module '{0}' has failed", module.Name));
                if(status == 2)
                    Rollback(moduleType);
                return false;
            }
            else
            {
                _writers.Standard.WriteLine(string.Format("Module '{0}' has been installed.", module.Name));
                return true;
            }            
        }

        private void Rollback(Type moduleType)
        {
            if(!new ModuleUninstaller(_commands, _modules, _writers).Uninstall(moduleType))
                _writers.Error.WriteLine("Because the module could not be uninstalled, the application may be unstable until it is restarted");
            else
                _writers.Standard.WriteLine("The module installation has been rolled back successfully");
        }

        // 0 = success; 1 = failed at validation before any changes were committed; 2 = partial install before failure 
        private int Install_INTERNAL(Type moduleType)
        {
            int status = 1;

            try
            {
                // see if module is already installed and exit with error if it is

                var module = CreateAndValidateModule(moduleType);
                if (module == null)
                    return status;

                _writers.Verbose.WriteLine(string.Format("Loaded module '{0}' from type '{1}'"
                    , module.Name, moduleType));

                // register command types and exit with errors if any occur

                status = 2;

                if (!RegisterModuleCommands(module))
                    return status;

                // add the module to the installed modules list - changes have been made at this point and must be rolled back

                _modules.Add(module);

                // install referenced modules - no need to exit with errors when a referenced module fails

                if (!InstallReferencedModules(module))
                    return status;

                // try to install the module

                try
                {
                    module.Install();
                    return 0;
                }
                catch (Exception ex) // unhandled error from failure TODO: handle error (log...)
                {
                    string installErrorMessage = string.Format("Module '{0}' encountered an unhandled exception during in its install logic"
                            , module.Name);
                    _writers.Error.WriteLine(installErrorMessage);
                    return status;
                }
            }
            catch (Exception ex) // handle any unexpected exceptions
            {
                string message = string.Format("An unexpected error occured during the installation of module type '{0}' :: {1}", moduleType, ex.Message);
                _writers.Error.WriteLine(message);
                return status;
            }
        }

        private bool InstallReferencedModules(IModule module)
        {
            if (module.Dependencies != null)
            {
                foreach (var dependency in module.Dependencies)
                {
                    var depModule = (IModule)Activator.CreateInstance(dependency);

                    switch (new ModuleValidator(_modules).Validate(dependency))
                    {
                        case ModuleValidator.ModuleValidationResult.Valid:
                            _writers.Verbose.WriteLine(string.Format("Installing referenced module '{0}' from assembly '{1}' for module '{2}' ...",
                                depModule.Name, dependency.Assembly, module.GetType()));
                            var status = Install_INTERNAL(dependency);
                            if (status != 0)
                            {
                                _writers.Error.WriteLine("Because the referenced module failed to install, the referenced module installation will be rolled back.");
                                if(status == 2)
                                    if (!new ModuleUninstaller(_commands, _modules, _writers).Uninstall(dependency))
                                        _writers.Error.WriteLine("Because the referenced module failed to uninstall, the application may be unstable until it is restarted");
                                return false;
                            }
                            break;
                        case ModuleValidator.ModuleValidationResult.AlreadyInstalled:
                            _writers.Verbose.WriteLine("Referenced module '{0}' from assembly '{1}' is already installed and will be skipped"
                                , depModule.Name, dependency.Assembly);
                            break;
                        case ModuleValidator.ModuleValidationResult.TypeInstalledFromAnotherAssembly:
                            _writers.Verbose.WriteLine("Referenced module '{0}' from assembly '{1}' is already installed but from a different assembly."
                                , depModule.Name, dependency.Assembly);
                            return false;
                        case ModuleValidator.ModuleValidationResult.DuplicateName:
                            var dependencyModule = (IModule)Activator.CreateInstance(dependency);
                            _writers.Verbose.WriteLine("Referenced module '{0}' from assembly '{1}' named '{2}' has a name that is already being used."
                                , depModule.Name, dependency.Assembly, dependencyModule.Name);
                            return false;
                        case ModuleValidator.ModuleValidationResult.CircularReference:
                            _writers.Verbose.WriteLine("Referenced module '{0}' from assembly '{1}' has a circular reference in it's dependency chain."
                                , depModule.Name, dependency.Assembly);
                            return false;
                    }
                }
            }

            return true;
        }

        private IModule CreateAndValidateModule(Type moduleType)
        {
            string uninstallableMessage = null;
            IModule module = (IModule)Activator.CreateInstance(moduleType);

            switch (new ModuleValidator(_modules).Validate(moduleType))
            {
                case ModuleValidator.ModuleValidationResult.AlreadyInstalled:
                    uninstallableMessage
                        = string.Format("The module type '{0}' has already been loaded - installation of this module has been aborted.", moduleType);
                    break;
                case ModuleValidator.ModuleValidationResult.TypeInstalledFromAnotherAssembly:
                    uninstallableMessage
                        = string.Format("The module type '{0}' has already been loaded from a different assembly - installation of this module has been aborted.", moduleType);
                    break;
                case ModuleValidator.ModuleValidationResult.DuplicateName:
                    uninstallableMessage
                        = string.Format("A module with the name '{0}' is already loaded - installation of this module has been aborted.", module.Name);
                    break;
                case ModuleValidator.ModuleValidationResult.CircularReference:
                    uninstallableMessage
                        = string.Format("The module type '{0}' has a circular reference in its dependency tree - installation of this module has been aborted.", moduleType);
                    break;
            }

            if (uninstallableMessage != null)
            {
                _writers.Verbose.WriteLine(uninstallableMessage);
                return null;
            }

            return module;
        }

        private bool RegisterModuleCommands(IModule module)
        {
            try
            {
                if (module.CommandTypes != null)
                    _commands.Register(module.CommandTypes.ToArray());
            }
            catch (Exception ex)
            {
                string message = string.Format("The commands for module '{0}' encountered an error during registration - {1}"
                    , module.Name, ex.Message);
                _writers.Error.WriteLine(message);
                return false;
            }

            return true;
        }

    }
}
