using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BitPantry.Theta.Modules.Variables
{
    [XmlRoot]
    public class VariableContextCollection
    {
        [XmlArray]
        [XmlArrayItem("Add")]
        public List<VariableContext> Contexts { get; set; }

        public VariableContextCollection()
        {
            Contexts = new List<VariableContext>();
        }
    }
}
