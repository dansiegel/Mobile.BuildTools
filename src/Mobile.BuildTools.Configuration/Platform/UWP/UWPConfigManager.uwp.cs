using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mobile.BuildTools.Configuration
{
    internal sealed class UWPConfigManager : IPlatformConfigManager
    {
        public IEnumerable<string> GetEnvironments()
        {
            var assets = Directory.GetFiles("Assets", "app.*.config")
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
            var assets = Directory.GetFiles("Assets", "*.config", SearchOption.AllDirectories);
            path = assets.FirstOrDefault(x => Path.GetFileName(x) == name || Path.GetFileName(x) == $"app.{name}.config");
            return !string.IsNullOrEmpty(path);
        }
    }
}