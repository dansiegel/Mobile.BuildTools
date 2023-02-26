using Windows.ApplicationModel;

namespace Mobile.BuildTools.Configuration;

internal sealed class WinUIConfigManager : IPlatformConfigManager
{
    public IEnumerable<string> GetEnvironments()
    {
        var assets = Directory.GetFiles(GetBasePath(), "app.*.config", SearchOption.AllDirectories)
            .Where(x => Path.GetExtension(x).ToLower() == ".config" &&
                    !Path.GetFileName(x).Equals("app.config"))
            .Select(x => Path.GetFileName(x));
        return assets.Select(x => x.Split('.')[1]).ToList();
    }

    public StreamReader GetStreamReader(string name)
    {
        if (ResourceExists(name, out var path))
            return new StreamReader(path);

        return StreamReader.Null;
    }

    public bool ResourceExists(string name, out string path)
    {
        var assets = Directory.GetFiles(GetBasePath(), "*.config", SearchOption.AllDirectories);
        path = assets.FirstOrDefault(x => Path.GetFileName(x) == name || Path.GetFileName(x) == $"app.{name}.config");
        return !string.IsNullOrEmpty(path);
    }

    private static string GetBasePath()
    {
        if (IsPackagedApp())
            return Package.Current.InstalledLocation.Path;

        return AppContext.BaseDirectory;
    }

    private static bool IsPackagedApp()
    {
        try
        {
            if (Package.Current != null)
                return true;
        }
        catch
        {
            // no-op
        }

        return false;
    }
}
