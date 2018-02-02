using BitPantry.Theta.API;
using BitPantry.Theta.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component
{
    /// <summary>
    /// A collection of input commands
    /// </summary>
    public class CommandCollection : IReadOnlyList<InputCommandDef>
    {
        private object locker = new object();

        #region READ ONLY LIST

        private List<Type> _commands = null;
        private List<InputCommandDef> Definitions
        {
            get
            {
                if (this._commands.Count == 0)
                    return new List<InputCommandDef>();
                return _commands.Select(t => t.DescribeInputCommand()).ToList();
            }
        }

        public InputCommandDef this[int index]
        {
            get { return this.Definitions[index]; }
        }

        public int Count
        {
            get { return this.Definitions.Count; }
        }

        public IEnumerator<InputCommandDef> GetEnumerator()
        {
            return this.Definitions.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Definitions.GetEnumerator();
        }

        #endregion

        internal CommandCollection()
        {
            this._commands = new List<Type>();
        }

        #region COMMAND REGISTRATION

        /// <summary>
        /// Registers the given command types in the command shell
        /// </summary>
        /// <param name="commands">Input command types to be registerd</param>
        /// <remarks>All commands are registered together - so that, if one has an issue, none are registered</remarks>
        public void Register(params Type[] commands)
        {
            lock (this.locker)
            {
                // validate commands before adding them

                foreach (var type in commands)
                {
                    if (this._commands.Contains(type))
                        throw new Exception(string.Format("The command type '{0}' is already registered.", type));
                        
                    this.ValidateCommand(type);
                } 
       
                // register commands

                foreach (var type in commands)
                    this._commands.Add(type);
            }
        }

        private void ValidateCommand(Type type)
        {
            InputCommandDef definition = type.DescribeInputCommand();
            List<string> commandNames = new List<string>();
            commandNames.Add(definition.CommandName);
            commandNames.AddRange(definition.Aliases);

            foreach (var def in this.Definitions)
            {
                List<string> regCommandNames = new List<string>();
                regCommandNames.Add(def.CommandName);
                regCommandNames.AddRange(def.Aliases);

                if(commandNames.Except(regCommandNames, StringComparer.OrdinalIgnoreCase).Count() != commandNames.Count()) 
                    throw new InvalidCommandException(definition.InputCommandType
                        , string.Format("Command {0} has a conflicting command name or alias with command {1}"
                        , definition.InputCommandType, def.InputCommandType));
            }               
        }

        /// <summary>
        /// Removes (unregisters) the given commands from the command shell
        /// </summary>
        /// <param name="commands">The commands types to remove from the command shell</param>
        /// <remarks>If the commands are part of a module, an exception will be thrown</remarks>
        public void Unregister(params Type[] commands)
        {
            lock (this.locker)
            {
                foreach (var type in commands)
                    this._commands.Remove(type);
            }
        }


        #endregion


        
    }
}
