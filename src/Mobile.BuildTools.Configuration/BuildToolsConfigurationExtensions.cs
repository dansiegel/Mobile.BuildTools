using Microsoft.Extensions.Configuration;

namespace Mobile.BuildTools.Configuration;

public static class BuildToolsConfigurationExtensions
{
    public static IConfigurationBuilder AddAppConfig(this IConfigurationBuilder builder, bool enableRuntimeEnvironments)
    {
        ConfigurationManager.Init(enableRuntimeEnvironments);
        return AddAppConfig(builder, ConfigurationManager.Current);
    }

#if ANDROID
    public static IConfigurationBuilder AddAppConfig(this IConfigurationBuilder builder, bool enableRuntimeEnvironments, Android.Content.Context context)
    {
        ConfigurationManager.Init(enableRuntimeEnvironments, context);
        return AddAppConfig(builder, ConfigurationManager.Current);
    }
#endif

    public static IConfigurationBuilder AddAppConfig(this IConfigurationBuilder builder, IConfigurationManager manager)
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
