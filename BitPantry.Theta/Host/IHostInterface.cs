using BitPantry.Theta.API;
using BitPantry.Theta.Component;
using BitPantry.Theta.Component.Modules;
using BitPantry.Theta.Component.Writers;

namespace BitPantry.Theta.Host
{
    public enum HostInterfaceMode
    {
        Output,
        Interactive
    }

    public interface IHostInterface 
    {
        HostInterfaceMode Mode { get; set; }
        IBufferedWriter GetBuffer();
        HostWriterCollection Out { get; }
        CommandCollection Commands { get; }
        ModuleCollection Modules { get; }
        ICommandActivatorContainer CommandActivatorContainer { get; }


        string Prompt(string prompt);
        void Clear();
    }
}
