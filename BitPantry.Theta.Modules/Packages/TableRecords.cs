using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Packages
{
    class TableRecords
    {
        #region PACKAGE

        public static List<dynamic> CreatePackageRecordList(params Package[] packages)
        {
            List<dynamic> records = new List<dynamic>();

            if (packages != null && packages.Count() > 0)
            {
                foreach (var item in packages)
                {
                    records.Add(CreatePackageTableRecord(
                        item.Name,
                        PackageLogic.Instance.LoadedPackages.Contains(item) ? "Loaded" : "Not Loaded",
                        string.Join(System.Environment.NewLine, item.Modules.Select(m => m.Name))
                        ));
                }
            }
            else
            {
                records.Add(CreatePackageTableRecord(string.Empty, string.Empty, string.Empty));
            }

            return records;
        }

        private static dynamic CreatePackageTableRecord(string name, string status, string modules)
        {
            return new
            {
                Name = name,
                Status = status,
                Modules = modules
            };
        }

        #endregion

        #region PACKAGE MODULE

        public static List<dynamic> CreatePackageModuleTableRecordList(params PackageModule[] modules)
        {
            List<IModule> moduleList = new List<IModule>();

            if (modules != null && modules.Count() > 0)
            {
                Dictionary<string, System.Reflection.Assembly> asmDict = new Dictionary<string, System.Reflection.Assembly>();

                foreach (var mod in modules)
                {
                    if (!asmDict.ContainsKey(mod.Assembly))
                        asmDict.Add(mod.Assembly, Util.LoadAssembly(mod.Assembly));
                    var asm = asmDict[mod.Assembly];

                    var type = asm.GetType(mod.ModuleType);
                    var iMod = (IModule)Activator.CreateInstance(type);

                    moduleList.Add(iMod);
                }

                return Theta.Component.Modules.TableRecords.CreateModuleTableRecordList(moduleList.ToArray());
            }
            else
            {
                return Theta.Component.Modules.TableRecords.CreateModuleTableRecordList();
            }
        }

        #endregion



    }
}
