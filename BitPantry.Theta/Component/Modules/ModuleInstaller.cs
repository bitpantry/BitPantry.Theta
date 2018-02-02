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
            this._commands = commands;
            this._modules = modules;
            this._writers = writers;
        }

        public bool Install(Type moduleType)
        {
            var module = (IModule)Activator.CreateInstance(moduleType);

            this._writers.Verbose.WriteLine(string.Format("Installing module type '{0}' from assembly '{1}' ..."
                , moduleType, moduleType.Assembly));

            var status = this.Install_INTERNAL(moduleType);

            if (status > 0)
            {
                this._writers.Error.WriteLine(string.Format("The installation of module '{0}' has failed", module.Name));
                if(status == 2)
                    this.Rollback(moduleType);
                return false;
            }
            else
            {
                this._writers.Standard.WriteLine(string.Format("Module '{0}' has been installed.", module.Name));
                return true;
            }            
        }

        private void Rollback(Type moduleType)
        {
            if(!new ModuleUninstaller(this._commands, this._modules, this._writers).Uninstall(moduleType))
                this._writers.Error.WriteLine("Because the module could not be uninstalled, the application may be unstable until it is restarted");
            else
                this._writers.Standard.WriteLine("The module installation has been rolled back successfully");
        }

        // 0 = success; 1 = failed at validation before any changes were committed; 2 = partial install before failure 
        private int Install_INTERNAL(Type moduleType)
        {
            int status = 1;

            try
            {
                // see if module is already installed and exit with error if it is

                var module = this.CreateAndValidateModule(moduleType);
                if (module == null)
                    return status;

                this._writers.Verbose.WriteLine(string.Format("Loaded module '{0}' from type '{1}'"
                    , module.Name, moduleType));

                // register command types and exit with errors if any occur

                status = 2;

                if (!this.RegisterModuleCommands(module))
                    return status;

                // add the module to the installed modules list - changes have been made at this point and must be rolled back

                this._modules.Add(module);

                // install referenced modules - no need to exit with errors when a referenced module fails

                if (!this.InstallReferencedModules(module))
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
                    this._writers.Error.WriteLine(installErrorMessage);
                    return status;
                }
            }
            catch (Exception ex) // handle any unexpected exceptions
            {
                string message = string.Format("An unexpected error occured during the installation of module type '{0}' :: {1}", moduleType, ex.Message);
                this._writers.Error.WriteLine(message);
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

                    switch (new ModuleValidator(this._modules).Validate(dependency))
                    {
                        case ModuleValidator.ModuleValidationResult.Valid:
                            this._writers.Verbose.WriteLine(string.Format("Installing referenced module '{0}' from assembly '{1}' for module '{2}' ...",
                                depModule.Name, dependency.Assembly, module.GetType()));
                            var status = this.Install_INTERNAL(dependency);
                            if (status != 0)
                            {
                                this._writers.Error.WriteLine("Because the referenced module failed to install, the referenced module installation will be rolled back.");
                                if(status == 2)
                                    if (!new ModuleUninstaller(this._commands, this._modules, this._writers).Uninstall(dependency))
                                        this._writers.Error.WriteLine("Because the referenced module failed to uninstall, the application may be unstable until it is restarted");
                                return false;
                            }
                            break;
                        case ModuleValidator.ModuleValidationResult.AlreadyInstalled:
                            this._writers.Verbose.WriteLine("Referenced module '{0}' from assembly '{1}' is already installed and will be skipped"
                                , depModule.Name, dependency.Assembly);
                            break;
                        case ModuleValidator.ModuleValidationResult.TypeInstalledFromAnotherAssembly:
                            this._writers.Verbose.WriteLine("Referenced module '{0}' from assembly '{1}' is already installed but from a different assembly."
                                , depModule.Name, dependency.Assembly);
                            return false;
                        case ModuleValidator.ModuleValidationResult.DuplicateName:
                            var dependencyModule = (IModule)Activator.CreateInstance(dependency);
                            this._writers.Verbose.WriteLine("Referenced module '{0}' from assembly '{1}' named '{2}' has a name that is already being used."
                                , depModule.Name, dependency.Assembly, dependencyModule.Name);
                            return false;
                        case ModuleValidator.ModuleValidationResult.CircularReference:
                            this._writers.Verbose.WriteLine("Referenced module '{0}' from assembly '{1}' has a circular reference in it's dependency chain."
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

            switch (new ModuleValidator(this._modules).Validate(moduleType))
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
                this._writers.Verbose.WriteLine(uninstallableMessage);
                return null;
            }

            return module;
        }

        private bool RegisterModuleCommands(IModule module)
        {
            try
            {
                if (module.CommandTypes != null)
                    this._commands.Register(module.CommandTypes.ToArray());
            }
            catch (Exception ex)
            {
                string message = string.Format("The commands for module '{0}' encountered an error during registration - {1}"
                    , module.Name, ex.Message);
                this._writers.Error.WriteLine(message);
                return false;
            }

            return true;
        }

    }
}
