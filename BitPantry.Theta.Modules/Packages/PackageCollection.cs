using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BitPantry.Theta.Modules.Packages
{
    [XmlRoot("Packages")]
    public class PackageCollection
    {
        [XmlArray]
        [XmlArrayItem(ElementName="Add")]
        public List<Package> Packages { get; set; }

        public PackageCollection()
        {
            this.Packages = new List<Package>();
        }


    }
}
