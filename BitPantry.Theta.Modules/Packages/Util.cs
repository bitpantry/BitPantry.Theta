using BitPantry.Theta.Component.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPantry.Theta.Extensions;
using System.IO;
using BitPantry.Theta.API;
using System.Reflection;

namespace BitPantry.Theta.Modules.Packages
{
    class Util
    {
        public static Assembly LoadAssembly(string assembly, IWriterCollection writers=null)
        {
            if (File.Exists(assembly))
            {
                string fileName = new FileInfo(assembly).FullName;
                if(writers != null)
                    writers.Debug.WriteLine(string.Format("Loading assembly from file {0}", fileName));
                return Assembly.LoadFile(fileName);
            }
            else
            {
                if(writers != null)
                    writers.Debug.WriteLine(string.Format("Loading assembly from assembly string '{0}'", assembly));
                return Assembly.Load(assembly);
            }
        }

    }
}
