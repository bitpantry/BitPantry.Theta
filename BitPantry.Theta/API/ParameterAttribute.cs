using System;
using System.Collections.Generic;

namespace BitPantry.Theta.API
{
    /// <summary>
    /// This attribute decorates input command properties to define named parameters for the command
    /// </summary>
    /// <remarks>This attribute may only be applied to a property</remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class Parameter : Attribute
    {
        /// <summary>
        /// The name of the named parameter
        /// </summary>
        /// <remarks>If no name is defined, the name of the property will be used</remarks>
        public string ParameterName { get; set; }

        /// <summary>
        /// Whether or not the parameter is required
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// The ordinal position of the parameter if the value is passed in as ordinal
        /// </summary>
        public int OrdinalPosition { get; set; }

        /// <summary>
        /// The name of the parameter set that this parameter belongs to
        /// </summary>
        public string ParameterSet { get; set; }

        /// <summary>
        /// The name of the function within the command class where a parameter's auto complete values can be generated
        /// </summary>
        public string AutoCompleteValuesFunction { get; set; }


        /// <summary>
        /// The name of the function within the command where the parameter value can be validated
        /// </summary>
        public string ValidationFunction { get; set; }

        /// <summary>
        /// Whether or not to use the auto complete values for validation
        /// </summary>
        /// <remarks>If true, an auto complete function must be defined and a validation function must not be defined</remarks>
        public bool UseAutoCompleteForValidation { get; set; }

        
        /// <summary>
        /// Instantiates an instance of the <paramref name="BitPantry.Console.API.InputCommandNamedParameter"/> object.
        /// </summary>
        /// <param name="parameterName">The name of the parameter</param>
        /// <param name="isRequired">Whether or not the parameter is required</param>
        /// <param name="isValueRequired">Whether or not a value other than an empty string is required</param>
        /// <param name="ordinalPosition">The ordinal position of the parameter if the value is passed ordinally</param>
        public Parameter(string parameterName=null, bool isRequired=true, int ordinalPosition=-1, string parameterSet=null
            , string autoCompleteValuesFunction=null, bool useAutoCompleteForValidation=false, string validationFunction=null)
        {
            this.ParameterName = parameterName;
            this.IsRequired = isRequired;
            this.OrdinalPosition = ordinalPosition;
            this.ParameterSet = parameterSet;
            this.AutoCompleteValuesFunction = autoCompleteValuesFunction;
            this.UseAutoCompleteForValidation = useAutoCompleteForValidation;
            this.ValidationFunction = validationFunction;
        }

        
    }
}
