using BitPantry.Theta.API;
using BitPantry.Theta.Component;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Core.Commands
{
    [Command("InstallModule")]
    [Alias("imod")]
    [Synopsis("Installs modules from an assembly")]
    class InstallModuleCommand : InputCommand
    {
        [Parameter(ordinalPosition: 1, autoCompleteValuesFunction:"GetAssemblyFilenameAutoCompleteValues")]
        [Synopsis("The name of a globally located assembly or the file path to the assembly for which to install all modules")]
        public string Assembly { get; set; }

        [Parameter(ordinalPosition: 2, autoCompleteValuesFunction: "GetModuleNamesAutoComplete", isRequired: false)]
        [Synopsis("A single module name or a comma seperated list of module names within the assembly to install - if module names are not provided, all modules found in the assembly will be installed")]
        public string Modules { get; set; }

        public override void Invoke(CommandInvocationContext context)
        {
            var asm = LoadAssembly();

            List<string> modulesToInstall = null;
            if (!string.IsNullOrWhiteSpace(Modules))
                modulesToInstall = new List<string>(Modules.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

            bool success = true;
            int count = 0;

            foreach (var type in asm.GetTypes())
            {
                if (type.GetInterface(typeof(IModule).FullName) != null)
                {
                    var module = (IModule)Activator.CreateInstance(type);
                    bool install = true;
                    if (modulesToInstall != null && !modulesToInstall.Contains(module.Name, StringComparer.OrdinalIgnoreCase))
                        install = false;

                    if (install)
                    {
                        base.Out.Verbose.WriteLine(string.Format("Loading module '{0}' from type '{1}'", module.Name, type));
                        if (!base.Host.Modules.Install(type, base.Out) && success)
                            success = false;
                        count++;
                    }
                }
            }

            if (success)
            {
                if (count > 0)
                    base.Out.Standard.WriteLine("Modules loaded successfully");
                else
                    base.Out.Standard.WriteLine("No modules could be found to load");
            }
            else
            {
                base.Out.Warning.WriteLine("The module installation finished with errors");
            }
        }

        private System.Reflection.Assembly LoadAssembly()
        {
            System.Reflection.Assembly asm = null;
            if (File.Exists(Assembly))
            {
                string fileName = new FileInfo(Assembly).FullName;
                base.Out.Verbose.WriteLine(string.Format("Loading assembly from file {0}", fileName));
                asm = System.Reflection.Assembly.LoadFile(fileName);
            }
            else
            {
                base.Out.Verbose.WriteLine(string.Format("Loading assembly from assembly string '{0}'", Assembly));
                asm = System.Reflection.Assembly.Load(Assembly);
            }

            if (asm == null)
                throw new Exception(string.Format("No assembly could be loaded from assembly string '{0}'", Assembly));

            return asm;
        }

        public void GetModuleNamesAutoComplete(AutoCompleteValuesFunctionContext context)
        {
            if (!string.IsNullOrWhiteSpace(Assembly))
            {
                var asm = LoadAssembly();
                context.Values.AddRange(asm.GetTypes().Where(t => t.GetInterface(typeof(IModule).FullName) != null).Select(t => ((IModule)Activator.CreateInstance(t)).Name));
            }
        }

        public void GetAssemblyFilenameAutoCompleteValues(AutoCompleteValuesFunctionContext context)
        {
            context.Values.AddRange(new UserAssemblyRepository().Assemblies.Select(a => a.Name));
        }

        public override List<string> GetDetailsDocumentation()
        {
            return new List<string>()
            {
                "When loading modules by assembly, the fully qualified assembly name can be used (if it is",
                "located where it can be resolved globally - i.e., the GAC), or the file path may be used.",
                "",
                "For the Types parameter, the types must be comma seperated and fully qualified within the",
                "assembly. They do not need to be case sensitive."
            };
        }
    }
}
