using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mobile.BuildTools.Configuration
{
    internal sealed class CommonConfigManager : IPlatformConfigManager
    {
        public IEnumerable<string> GetEnvironments()
        {
            var assets = Directory.GetFiles(Directory.GetCurrentDirectory(), "app.*.config")
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
            if(!Path.HasExtension(name))
            {
                name = name == "app" ? "app.config" : $"app.{name}.config";
            }

            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.config", SearchOption.AllDirectories);
            path = files.FirstOrDefault(x => Path.GetFileName(x) == name);
            return !string.IsNullOrEmpty(path);
        }
    }
}
