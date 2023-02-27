using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mobile.BuildTools.Configuration;

public static class BuildToolsConfigurationExtensions
{
    public static IHostBuilder AddBuildToolsConfiguration(this IHostBuilder host, bool enableRuntimeEnvironments)
    {
        var manager = ConfigurationManager.Init(enableRuntimeEnvironments);
        return host.ConfigureHostConfiguration(c => c.AddBuildToolsConfiguration(manager))
            .ConfigureServices((_, s) => s.AddSingleton<IConfigurationManager>(manager));
    }

    public static IConfigurationBuilder AddBuildToolsConfiguration(this IConfigurationBuilder builder, bool enableRuntimeEnvironments)
    {
        var manager = ConfigurationManager.Init(enableRuntimeEnvironments);
        return AddBuildToolsConfiguration(builder, manager);
    }

#if ANDROID
    public static IHostBuilder AddBuildToolsConfiguration(this IHostBuilder host, bool enableRuntimeEnvironments, Android.Content.Context context)
    {
        var manager = ConfigurationManager.Init(enableRuntimeEnvironments, context);
        return host.ConfigureHostConfiguration(c => c.AddBuildToolsConfiguration(manager))
            .ConfigureServices((_, s) => s.AddSingleton<IConfigurationManager>(manager));
    }

    public static IConfigurationBuilder AddBuildToolsConfiguration(this IConfigurationBuilder builder, bool enableRuntimeEnvironments, Android.Content.Context context)
    {
        var manager = ConfigurationManager.Init(enableRuntimeEnvironments, context);
        return AddBuildToolsConfiguration(builder, manager);
    }
#endif

    public static IConfigurationBuilder AddBuildToolsConfiguration(this IConfigurationBuilder builder, IConfigurationManager manager)
    {
        return builder.Add(new AppConfigSource(manager));
    }

    internal static IDictionary<string, string> ToData(this IConfigurationManager manager)
    {
        var data = new Dictionary<string, string>();
        manager.AppSettings.ToList().ForEach(x => data.Add(x.Key, x.Value));
        manager.ConnectionStrings.ToList().ForEach(x => data.Add($"ConnectionStrings:{x.Name}", x.ConnectionString));

        return data;
    }
}
