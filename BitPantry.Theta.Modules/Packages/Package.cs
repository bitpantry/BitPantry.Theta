using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BitPantry.Theta.Modules.Packages
{
    [XmlRoot]
    public class Package
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlArray("Assemblies")]
        [XmlArrayItem("Add")]
        public List<PackageModule> Modules { get; set; }

        public Package()
        {
            this.Modules = new List<PackageModule>();
        }
    }
}
