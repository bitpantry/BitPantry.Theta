using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.API
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Switch : Attribute
    {
        /// <summary>
        /// The name of the switch - if left default, the property name will be used
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Instantiates a new Switch instance
        /// </summary>
        /// <param name="name">The name of the switch - if left default, the property name will be used</param>
        public Switch(string name = null)
        {

        }
    }
}
