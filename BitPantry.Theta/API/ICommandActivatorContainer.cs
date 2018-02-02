using System;
using BitPantry.Theta.Processing;

namespace BitPantry.Theta.API
{
    public interface ICommandActivatorContainer
    {
        InputCommand Get(Type inputCommandType);

    }
}
