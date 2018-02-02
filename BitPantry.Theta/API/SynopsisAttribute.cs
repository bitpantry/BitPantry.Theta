using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.API
{
    /// <summary>
    /// Decorates a class or property to provide a synopsis
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple=false)]
    public class Synopsis : Attribute
    {
        /// <summary>
        /// The synopsis value
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Creates an instance of the Synopsis class
        /// </summary>
        /// <param name="value">The value of the synopsis</param>
        public Synopsis(string value) { Value = value; }

    }
}
