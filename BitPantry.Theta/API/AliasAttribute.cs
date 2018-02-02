using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.API
{
    /// <summary>
    /// Defines aliases for commands
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple=true)]
    public class Alias : Attribute
    {
        /// <summary>
        /// The alias
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Creates an instance of the Alias class
        /// </summary>
        /// <param name="alias">The alias</param>
        public Alias(string alias)
        {
            this.Value = alias;
        }
    }
}
