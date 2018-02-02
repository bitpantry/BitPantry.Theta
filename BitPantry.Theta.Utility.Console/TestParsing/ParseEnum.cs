using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPantry.Theta.API;

namespace BitPantry.Theta.Utility.Console.TestParsing
{
    public enum TestEnumType
    {
        One,
        Two
    }

    [Command]
    public class ParseEnum : InputCommand
    {
        [Parameter(OrdinalPosition = 1, AutoCompleteValuesFunction = "GetEnumNames", UseAutoCompleteForValidation = true)]
        public TestEnumType EnumArg { get; set; }
        public override void Invoke(CommandInvocationContext context)
        {
            Out.Standard.WriteLine(EnumArg);
        }

        public void GetEnumNames(AutoCompleteValuesFunctionContext ctx)
        {
            ctx.Values.AddRange(Enum.GetNames(typeof(TestEnumType)));
        }
    }
}
