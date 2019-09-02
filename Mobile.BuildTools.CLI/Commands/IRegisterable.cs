using McMaster.Extensions.CommandLineUtils;

namespace Mobile.BuildTools.Commands
{
    public interface IRegisterable
    {
        void Register(CommandLineApplication app);
    }
}
