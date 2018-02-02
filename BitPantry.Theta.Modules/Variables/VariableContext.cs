using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BitPantry.Theta.Modules.Variables
{
    [XmlRoot]
    public class VariableContext
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

        [XmlArray]
        [XmlArrayItem("Add")]
        public List<VariableContextVariable> Variables { get; set; }

        public VariableContext()
        {
            this.Variables = new List<VariableContextVariable>();
        }
    }
}
