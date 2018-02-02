using System;
using System.Collections.Generic;
using System.Reflection;

namespace BitPantry.Theta.API
{
    /// <summary>
    /// Describes an input command named parameter
    /// </summary>
    public class ParameterDef : ISwitchDefinition
    {
        /// <summary>
        /// The property descriptor for the named parameter property
        /// </summary>
        public PropertyInfo PropertyInfo { get; internal set; }

        /// <summary>
        /// The parameter name
        /// </summary>
        /// <remarks>If no name is defined in the attribute, this will be the name of the property</remarks>
        public string Name { get; internal set; }

        private List<string> _aliases = new List<string>();
        /// <summary>
        /// The list of aliases
        /// </summary>
        public IEnumerable<string> Aliases { get { return _aliases.AsReadOnly(); } }

        /// <summary>
        /// Whether or not the named parameter is required
        /// </summary>
        public bool IsRequired { get; internal set; }

        /// <summary>
        /// The ordinal position of the parameter when the value is passed ordinally
        /// </summary>
        public int OrdinalPosition { get; internal set; }

        /// <summary>
        /// A synopsis of the parameter
        /// </summary>
        public string Synopsis { get; internal set; }

        /// <summary>
        /// The name of the parameter set to which this parameter belongs
        /// </summary>
        public string ParameterSet { get; internal set; }

        /// <summary>
        /// The name of the function within the command class where a parameter's auto complete values can be generated
        /// </summary>
        public string AutoCompleteValuesFunction { get; internal set; }

        /// <summary>
        /// The name of the function within the command where the parameter value can be validated
        /// </summary>
        public string ValidationFunction { get; internal set; }

        /// <summary>
        /// Whether or not to use the auto complete values for validation
        /// </summary>
        /// <remarks>If true, an auto complete function must be defined and a validation function must not be defined</remarks>
        public bool UseAutoCompleteForValidation { get; internal set; }


        #region ADD FUNCTIONS

        internal void AddAliases(IEnumerable<string> aliases)
        {
            _aliases.AddRange(aliases);
        }

        #endregion

    }
}
