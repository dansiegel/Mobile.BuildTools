using System.Collections.Generic;
using System.IO;
using System.Linq;
using Foundation;

namespace Mobile.BuildTools.Configuration
{
    internal sealed class AppleConfigManager : IPlatformConfigManager
    {
        public IEnumerable<string> GetEnvironments()
        {
            var assets = Directory.GetFiles(NSBundle.MainBundle.BundlePath, "app.*.config")
                .Where(x => Path.GetExtension(x).ToLower() == ".config" &&
                        !Path.GetFileName(x).Equals("app.config"))
                .Select(x => Path.GetFileName(x));
            return assets.Select(x => x.Split('.')[1]).ToList();
        }

        public StreamReader GetStreamReader(string name)
        {
            if (!ResourceExists(name, out var path))
                new StreamReader(Stream.Null);

            return new StreamReader(path);
        }

        public bool ResourceExists(string name, out string path)
        {
            if(GetEnvironments().Any(x => x.Equals(name, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                name = $"app.{name}.config";
            }

            path = Directory.GetFiles(NSBundle.MainBundle.BundlePath, name, SearchOption.AllDirectories)
                                .FirstOrDefault();

            return !string.IsNullOrEmpty(path);
        }
    }
}