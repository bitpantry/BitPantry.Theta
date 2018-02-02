using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BitPantry.Theta.Modules.Packages
{
    [XmlRoot]
    public class PackageModule
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string ModuleType { get; set; }

        [XmlAttribute]
        public string Assembly { get; set; } // string or path
    }
}
