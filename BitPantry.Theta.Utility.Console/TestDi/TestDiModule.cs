using System;
using System.Collections.Generic;
using BitPantry.Theta.API;

namespace BitPantry.Theta.Utility.Console.TestDi
{
    class TestDiModule : IModule
    {
        public string Name => "TestDiModule";
        public List<Type> CommandTypes => new List<Type> { typeof(TestDiCommand) };
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
