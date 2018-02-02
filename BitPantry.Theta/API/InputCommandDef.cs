using System;
using System.Collections.Generic;

namespace BitPantry.Theta.API
{
    /// <summary>
    /// Describes an input command
    /// </summary>
    /// <remarks>This class is used internally by the shell to parse and consistently define input commands from a type, and it
    /// may also be used by developers to do the same</remarks>
    public class InputCommandDef
    {
        /// <summary>
        /// Gets the type of the input command class
        /// </summary>
        public Type InputCommandType { get; internal set; }

        /// <summary>
        /// Gets the name of the input command
        /// </summary>
        /// <remarks>The CommandName is the name specified by the Command attribute or the name of the class if the command attribute is not defined</remarks>
        public string CommandName { get; internal set; }

        private List<string> _aliases = new List<string>();
        /// <summary>
        /// Gets the list of command aliases
        /// </summary>
        public IEnumerable<string> Aliases { get { return this._aliases.AsReadOnly(); } }


        private List<ParameterDef> _parameters = new List<ParameterDef>();
        /// <summary>
        /// Gets the list of descriptors for the parameters defined for the command type
        /// </summary>
        public IEnumerable<ParameterDef> Parameters { get { return this._parameters.AsReadOnly(); } }

        private List<SwitchDef> _switches = new List<SwitchDef>();
        /// <summary>
        /// Gets the list of descriptors fot the switches defined for the command
        /// </summary>
        public IEnumerable<SwitchDef> Switches { get { return this._switches.AsReadOnly(); } }

        /// <summary>
        /// A synopsis of the input command
        /// </summary>
        public string Synopsis { get; internal set; }

        /// <summary>
        /// The name of the default parameter set
        /// </summary>
        public string DefaultParameterSet { get; internal set; }

        /// <summary>
        /// Creates an instance of the <paramref name="BitPantry.Console.API.InputCommandDef"/> object
        /// </summary>
        public InputCommandDef() { }

        #region ADD FUNCTIONS

        internal void Add(IEnumerable<SwitchDef> switches)
        {
            this._switches.AddRange(switches);
        }

        internal void Add(IEnumerable<ParameterDef> parameters)
        {
            this._parameters.AddRange(parameters);
        }

        internal void AddAliases(IEnumerable<string> aliases)
        {
            this._aliases.AddRange(aliases);
        }

        #endregion
    }
}
