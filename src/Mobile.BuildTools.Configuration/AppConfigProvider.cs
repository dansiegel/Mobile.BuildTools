using Microsoft.Extensions.Configuration;

namespace Mobile.BuildTools.Configuration;

internal class AppConfigProvider : ConfigurationProvider
{
    private IConfigurationManager _manager { get; }
    public AppConfigProvider(IConfigurationManager manager)
    {
        _manager = manager;
        if (_manager.Environments.Count > 1)
            _manager.SettingsChanged += (s, e) =>
            {
                Load();
                OnReload();
            };
    }

    public override void Load()
    {
        Data = _manager.ToData();
    }
}
