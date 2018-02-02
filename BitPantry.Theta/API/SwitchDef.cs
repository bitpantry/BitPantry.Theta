using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.API
{
    public class SwitchDef : ISwitchDefinition
    {
        public PropertyInfo PropertyInfo { get; internal set; }
        public string Name { get; internal set; }
        public string Synopsis { get; internal set; }

        private List<string> _aliases = new List<string>();
        public IEnumerable<string> Aliases { get { return this._aliases.AsReadOnly(); } }

        #region ADD FUNCTIONS

        internal void AddAliases(IEnumerable<string> aliases)
        {
            this._aliases.AddRange(aliases);
        }

        #endregion
    }
}
