using BitPantry.Theta.API;
using BitPantry.Theta.Component.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component
{
    class AssemblyUtil
    {
        public static Assembly LoadAssembly(string assembly, IWriterCollection writers = null)
        {
            if (File.Exists(assembly))
            {
                string fileName = new FileInfo(assembly).FullName;
                if (writers != null)
                    writers.Debug.WriteLine(string.Format("Loading assembly from file {0}", fileName));
                return Assembly.LoadFile(fileName);
            }
            else
            {
                if (writers != null)
                    writers.Debug.WriteLine(string.Format("Loading assembly from assembly string '{0}'", assembly));
                return Assembly.Load(assembly);
            }
        }

        public static List<IModule> GetAssemblyModules(string assemblyStringOrPath, IWriterCollection writers=null)
        {
            List<Type> types = new List<Type>();
            var asm = LoadAssembly(assemblyStringOrPath, writers);
            foreach (var type in asm.GetTypes())
            {
                if (type.GetInterface(typeof(IModule).FullName) != null)
                    types.Add(type);
            }

            return types.Select(t => (IModule)Activator.CreateInstance(t)).ToList();
        }
    }
}
