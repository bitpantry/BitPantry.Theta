using Credera.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    class TestModule : IModule
    {
        public string Name { get { return "TestModule"; } }

        public List<Type> CommandTypes
        {
            get { return new List<Type>(); }
        }

        public List<Type> RequiredModuleTypes
        {
            get { return new List<Type>(); }
        }

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
