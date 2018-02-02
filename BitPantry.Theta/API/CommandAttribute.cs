using System;
using System.Collections.Generic;

namespace BitPantry.Theta.API
{
    /// <summary>
    /// The attribute defining an input command
    /// </summary>
    /// <remarks>Can be applied to a class only</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class Command : Attribute
    {
        /// <summary>
        /// The name of the command - if null, the class name will be used
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The name of the default parameter set
        /// </summary>
        public string DefaultParameterSet { get; set; }

        /// <summary>
        /// Initializes an instance of the Name attribute.
        /// </summary>
        /// <param name="name">The name of the command - if null, the class name will be used</param>
        public Command(string name=null, string defaultParameterSet=null) 
        {
            this.Name = name;
            this.DefaultParameterSet = defaultParameterSet;
        }
    }
}
