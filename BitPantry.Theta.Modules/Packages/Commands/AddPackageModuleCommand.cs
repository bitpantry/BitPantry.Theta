using BitPantry.Theta.API;
using BitPantry.Theta.Component;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Packages.Commands
{
    [Command("AddPackageModule")]
    [Synopsis("Adds a module to a package")]
    class AddPackageModuleCommand : InputCommand
    {
        [Parameter(ordinalPosition:1, autoCompleteValuesFunction:"GetPackageNamesAutoComplete", useAutoCompleteForValidation:true)]
        [Synopsis("The name of the package to add the module to")]
        public string Package { get; set; }

        [Parameter(ordinalPosition: 2, autoCompleteValuesFunction: "GetAssemblyFilenameAutoCompleteValues")]
        [Synopsis("The name of a globally located assembly or the file path to the assembly for which to install all modules")]
        public string Assembly { get; set; }

        [Parameter(ordinalPosition: 3, autoCompleteValuesFunction: "GetModuleNamesAutoComplete", isRequired: false)]
        [Synopsis("A single module name or a comma seperated list of module names within the assembly to add - if module names are not provided, all modules found in the assembly will be added")]
        public string Modules { get; set; }


        public override void Invoke(CommandInvocationContext context)
        {
            var asm = Util.LoadAssembly(Assembly);
            if (asm == null)
            {
                base.Out.Error.WriteLine(string.Format("Could not load assembly from assembly string '{0}'", Assembly));
                return;
            }              

            Package pkg = PackageLogic.Instance.PackagesCollection.Packages.FirstOrDefault(e => e.Name.Equals(Package, StringComparison.OrdinalIgnoreCase));

            List<string> modulesToInstall = null;
            if (!string.IsNullOrWhiteSpace(Modules))
                modulesToInstall = new List<string>(Modules.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

            int count = 0;

            foreach (var type in asm.GetTypes())
            {
                if (type.GetInterface(typeof(IModule).FullName) != null)
                {
                    var module = (IModule)Activator.CreateInstance(type);
                    bool add = true;
                    if (modulesToInstall != null && !modulesToInstall.Contains(module.Name, StringComparer.OrdinalIgnoreCase))
                        add = false;

                    if (add)
                    {
                        base.Out.Verbose.WriteLine(string.Format("Adding module '{0}' from type '{1}'", module.Name, type));
                        if (AddModule(pkg, module))
                            count++;
                    }
                }
            }

            PackageLogic.Instance.Save();

            if (count > 0)
            {
                base.Out.Object.Table(TableRecords.CreatePackageRecordList(pkg));
            }
            else
            {
                base.Out.Standard.WriteLine("No modules could be found to load");
            }
        }

        private bool AddModule(Packages.Package pkg, IModule module)
        {
            if (pkg.Modules.Any(m => m.Name.Equals(module.Name, StringComparison.OrdinalIgnoreCase)))
            {
                base.Out.Warning.WriteLine(string.Format("A module with the name '{0}' has already been added. The module will not be added.", module.Name));
            }
            else if (pkg.Modules.Any(m => m.ModuleType == module.GetType().FullName))
            {
                base.Out.Warning.WriteLine(string.Format("A module of type '{0}' has already been added. The module will not be added.", module.GetType()));
            }
            else
            {
                if (base.Host.Modules.Any(m => m.Name.Equals(module.Name, StringComparison.OrdinalIgnoreCase)))
                    base.Out.Warning.WriteLine(string.Format("A module with the name '{0}' is already installed. The package may not laod correctly now that this module is added.", module.Name));
                else if(base.Host.Modules.Any(m => m.GetType() == module.GetType()))
                    base.Out.Warning.WriteLine(string.Format("A module of type '{0}' is already installed. The package may not load correctly now that this module is added.", module.GetType()));

                pkg.Modules.Add(new PackageModule()
                {
                    Assembly = Assembly,
                    ModuleType = module.GetType().FullName,
                    Name = module.Name
                });

                return true;
            }

            return false;
        }

        public void GetPackageNamesAutoComplete(AutoCompleteValuesFunctionContext context)
        {
            context.Values.AddRange(PackageLogic.Instance.PackagesCollection.Packages.Select(e => e.Name));
        }

        public void GetAssemblyFilenameAutoCompleteValues(AutoCompleteValuesFunctionContext context)
        {
            context.Values.AddRange(new UserAssemblyRepository().Assemblies.Select(a => a.Name));
        }

        public void GetModuleNamesAutoComplete(AutoCompleteValuesFunctionContext context)
        {
            if (!string.IsNullOrWhiteSpace(Assembly))
            {
                var asm = Util.LoadAssembly(Assembly, base.Out);
                if (asm == null)
                    return;
                context.Values.AddRange(asm.GetTypes().Where(t => t.GetInterface(typeof(IModule).FullName) != null).Select(t => ((IModule)Activator.CreateInstance(t)).Name));
            }
        }
    }
}
