using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta
{
    public class InvalidCommandException : Exception
    {
        public Type InputCommandType { get; set; }

        public InvalidCommandException(Type inputCommandType, string message) : base(message)
        { this.InputCommandType = inputCommandType; }
    }
}
