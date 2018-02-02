using BitPantry.Theta.API;

namespace BitPantry.Theta.Utility.Console.TestDi
{
    [Command]
    public class TestDiCommand : InputCommand
    {
        private readonly TestDiDependency _dep;

        public TestDiCommand(TestDiDependency dep)
        {
            _dep = dep;
        }

        public override void Invoke(CommandInvocationContext context)
        {
            Out.Standard.WriteLine(_dep.Id);
        }
    }
}
