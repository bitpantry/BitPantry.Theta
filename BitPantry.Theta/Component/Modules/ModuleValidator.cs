using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component.Modules
{
    class ModuleValidator
    {
        public enum ModuleValidationResult
        {
            Valid,
            AlreadyInstalled,
            TypeInstalledFromAnotherAssembly,
            DuplicateName,
            CircularReference
        }

        private List<IModule> _modules = null;

        public ModuleValidator(List<IModule> modules)
        {
            this._modules = modules;
        }

        public ModuleValidationResult Validate(Type moduleType)
        {
            // check if module is already installed or installed from another or different assembly

            if (this._modules.Exists(m => m.GetType().FullName.Equals(moduleType.FullName)))
            {
                var installedModule = this._modules.FirstOrDefault(m => m.GetType().FullName.Equals(moduleType.FullName));
                if (installedModule.GetType().Assembly == moduleType.Assembly)
                    return ModuleValidationResult.AlreadyInstalled;
                else
                    return ModuleValidationResult.TypeInstalledFromAnotherAssembly;
            }

            // validate the module name is not duplicate

            IModule module = (IModule)Activator.CreateInstance(moduleType);

            if (this._modules.Exists(m => m.Name.Equals(module.Name, StringComparison.OrdinalIgnoreCase)))
                return ModuleValidationResult.DuplicateName;

            // validate that there are no circular references in the modules dependency hierearchy

            if (this.IsCircularReference(module, new List<Type>()))
                return ModuleValidationResult.CircularReference;

            // return valid

            return ModuleValidationResult.Valid;
        }

        private bool IsCircularReference(IModule module, List<Type> extensions)
        {
            extensions.Add(module.GetType());

            if (module.Dependencies != null)
            {
                foreach (var dependency in module.Dependencies)
                {
                    if (extensions.Contains(dependency))
                        return true;

                    if (this.IsCircularReference((IModule)Activator.CreateInstance(dependency), new List<Type>(extensions)))
                        return true;
                }
            }

            return false;
        }
    }
}
