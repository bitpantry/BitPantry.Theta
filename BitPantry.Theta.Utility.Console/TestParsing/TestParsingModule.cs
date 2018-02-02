using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPantry.Theta.API;

namespace BitPantry.Theta.Utility.Console.TestParsing
{
    class TestParsingModule : IModule
    {
        public string Name => "TestParsing";
        public List<Type> CommandTypes => new List<Type> { typeof(ParseEnum) };
        public List<Type> Dependencies => null;
        public void Install()
        {
            // do nothing
        }

        public void Uninstall()
        {
            // do nothing
        }
    }
}
