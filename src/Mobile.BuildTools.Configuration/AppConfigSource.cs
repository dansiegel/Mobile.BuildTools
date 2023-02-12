using Microsoft.Extensions.Configuration;

namespace Mobile.BuildTools.Configuration;

internal class AppConfigSource : IConfigurationSource
{
    private IConfigurationManager _manager { get; }

    public AppConfigSource(IConfigurationManager manager)
    {
        _manager = manager;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new AppConfigProvider(_manager);
    }
}
