using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component
{
    public class UserAssemblyRepository
    {
        public string UserAssemblyDirectoryPath
        {
            get
            {
                var template = string.Format(@"{0}\Theta\Assemblies\",
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
                var dir = new DirectoryInfo(template);
                if (!dir.Exists)
                    dir.Create();
                return dir.FullName;
            }
        }

        public IReadOnlyList<FileInfo> Assemblies
        {
            get
            {
                List<FileInfo> asmList = new List<FileInfo>();
                var dir = new DirectoryInfo(UserAssemblyDirectoryPath);
                asmList.AddRange(dir.GetFiles("*.dll"));
                asmList.AddRange(dir.GetFiles("*.exe"));
                return asmList.ToList().AsReadOnly();
            }
        }
    }
}
