using BitPantry.Theta.API;
using BitPantry.Theta.Component;
using BitPantry.Theta.Component.Modules;
using BitPantry.Theta.Component.Writers;
using BitPantry.Theta.Host;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BitPantry.Theta.Modules.Packages
{
    class PackageLogic
    {
        public const string FILE_PATH_PACKAGES = "packages.xml";

        public PackageCollection PackagesCollection { get; private set; }

        private List<string> _loadedPackageNames = new List<string>();
        public List<Package> LoadedPackages
        {
            get
            {
                return this.PackagesCollection.Packages
                    .Where(e => this._loadedPackageNames.Contains(e.Name, StringComparer.OrdinalIgnoreCase)).ToList();
            }
        }

        public List<Package> UnloadedPackages
        {
            get { return this.PackagesCollection.Packages.Where(e => !this.LoadedPackages.Contains(e)).ToList(); }
        }

        #region INITIALIZATION LOGIC

        private static PackageLogic _instance = null;
        public static PackageLogic Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PackageLogic();
                return _instance;
            }
        }

        private PackageLogic() 
        {
            this.DiscardChanges();
        }

        #endregion

        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PackageCollection));
            using (FileStream stream = File.Create(FILE_PATH_PACKAGES))
                serializer.Serialize(stream, this.PackagesCollection);
        }

        public void DiscardChanges()
        {
            if (new FileInfo(FILE_PATH_PACKAGES).Exists)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PackageCollection));
                using (FileStream stream = File.OpenRead(FILE_PATH_PACKAGES))
                    this.PackagesCollection = (PackageCollection)serializer.Deserialize(stream);
            }
            else
            {
                this.PackagesCollection = new PackageCollection();
                this.Save();
            }
        }

        public void Remove(Package package, ModuleCollection modules, IWriterCollection writers)
        {
            if (this.LoadedPackages.Contains(package))
                this.Unload(package, modules, writers);

            this.PackagesCollection.Packages.Remove(package);
        }

        public void Unload(Package package, ModuleCollection modules, IWriterCollection writers)
        {
            if (!this.LoadedPackages.Contains(package))
                throw new Exception(string.Format("The package {0} is not loaded", package.Name));

            Dictionary<string, System.Reflection.Assembly> asmDic = new Dictionary<string, System.Reflection.Assembly>();

            foreach (var module in package.Modules)
            {
                Type moduleType = null;
                System.Reflection.Assembly asm = null;

                try
                {
                    if (!asmDic.ContainsKey(module.Assembly))
                        asmDic.Add(module.Assembly, Util.LoadAssembly(module.Assembly));
                    asm = asmDic[module.Assembly];

                    moduleType = asm.GetType(module.ModuleType);
                    modules.Uninstall(moduleType, writers);
                    writers.Verbose.WriteLine(string.Format("Uninstalled module '{0}' from assembly '{1}'", moduleType, asm));
                }
                catch (Exception ex)
                {
                    writers.Error.WriteLine(string.Format("An error occured while uninstalling module '{0}' from assembly '{1}' :: {2}\r\n{3}"
                        , moduleType, asm.CodeBase.Replace("file:///", string.Empty), ex.Message, ex.StackTrace));

                    writers.Error.WriteLine("The application should be restarted to ensure that the package is fully unloaded.");
                }
            }

            this._loadedPackageNames.Remove(package.Name);
        }

        public void Load(Package package, ModuleCollection modules, IWriterCollection writers)
        {
            if (PackageLogic.Instance.LoadedPackages.Contains(package))
                throw new Exception(string.Format("The package {0} is already loaded", package.Name));

            Dictionary<string, System.Reflection.Assembly> asmDic = new Dictionary<string, System.Reflection.Assembly>();

            bool isErrors = false;

            if (package.Modules.Count > 0)
            {
                foreach (var module in package.Modules)
                {
                    Type moduleType = null;
                    System.Reflection.Assembly asm = null;

                    try
                    {
                        if (!asmDic.ContainsKey(module.Assembly))
                            asmDic.Add(module.Assembly, Util.LoadAssembly(module.Assembly));
                        asm = asmDic[module.Assembly];

                        moduleType = asm.GetType(module.ModuleType);
                        if (!modules.Install(moduleType, writers) && isErrors == false)
                            isErrors = true;
                    }
                    catch (Exception ex)
                    {
                        writers.Error.WriteLine(string.Format("An error occured while installing module '{0}' from assembly '{1}' :: {2}\r\n{3}"
                            , moduleType, asm.CodeBase.Replace("file:///", string.Empty), ex.Message, ex.StackTrace));

                        if (moduleType != null && modules.Any(m => m.GetType() == moduleType))
                            modules.Uninstall(moduleType, writers);

                        writers.Error.WriteLine("The module has been uninstalled.");

                        isErrors = true;
                    }
                }
            }
            else
            {
                writers.Warning.WriteLine("The package has been loaded but no modules were installed - the package does not define any modules.");
            }

            this._loadedPackageNames.Add(package.Name);

            if (isErrors)
                writers.Warning.WriteLine("The package has been loaded with errors.");
            else
                writers.Standard.WriteLine("Package loaded");
        }
    }
}
