using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component.Modules
{
    public class TableRecords
    {


        public static List<dynamic> CreateModuleTableRecordList(params IModule[] modules)
        {
            return CreateModuleTableRecordList(true, modules);
        }

        public static List<dynamic> CreateModuleTableRecordList(bool includeAssemblyInfo, params IModule[] modules)
        {
            List<dynamic> records = new List<dynamic>();

            if (modules != null && modules.Count() > 0)
            {
                foreach (var mod in modules)
                {
                    Type type = mod.GetType();

                    records.Add(CreateModuleTableRecord(
                        mod.Name,
                        type.FullName,
                        mod.Dependencies != null ? string.Join(Environment.NewLine, mod.Dependencies.Select(d => ((IModule)Activator.CreateInstance(d)).Name)) : string.Empty,
                        mod.CommandTypes != null ? string.Join(Environment.NewLine, mod.CommandTypes) : string.Empty,
                        includeAssemblyInfo
                            ? string.Format("{0}{1}{2}",
                                type.Assembly,
                                Environment.NewLine,
                                type.Assembly.CodeBase.Replace("file:///", string.Empty))
                            : null));

                }
            }
            else
            {
                records.Add(CreateModuleTableRecord(string.Empty, string.Empty, string.Empty, string.Empty, includeAssemblyInfo ? string.Empty : null));
            }

            return records;
        }

        private static dynamic CreateModuleTableRecord(string name, string type, string dependencies, string commands, string assembly)
        {
            if(assembly == null)
                return new
                {
                    Name = name,
                    Module = type,
                    Dependencies = dependencies,
                    Commands = commands
                };

            else
                return new
                        {
                            Name = name,
                            Module = type,
                            Dependencies = dependencies,
                            Commands = commands,
                            @Assembly =  assembly
                        };
        }
    }
}
