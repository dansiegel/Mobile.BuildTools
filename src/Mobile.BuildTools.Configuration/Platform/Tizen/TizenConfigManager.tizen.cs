using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tizen.Applications;

namespace Mobile.BuildTools.Configuration
{
    public sealed class TizenConfigManager : IPlatformConfigManager
    {

        public IEnumerable<string> GetEnvironments()
        {
            var files = Directory.GetFiles(Application.Current.DirectoryInfo.Resource, "*.config", SearchOption.AllDirectories)
                .Select(x => Path.GetFileName(x))
                .Where(x => x != "app.config" && x.StartsWith("app."));
            return files.Select(x => x.Split('.')[1]).ToList();
        }

        public StreamReader GetStreamReader(string name)
        {
            if (ResourceExists(name, out var path))
                return new StreamReader(path);

            return StreamReader.Null;
        }

        public bool ResourceExists(string name, out string path)
        {
            if (!Path.HasExtension(name))
            {
                name = name == "app" ? "app.config" : $"app.{name}.config";
            }

            var files = Directory.GetFiles(Application.Current.DirectoryInfo.Resource, name, SearchOption.AllDirectories);
            path = files.FirstOrDefault(x => Path.GetFileName(x) == name);
            return !string.IsNullOrEmpty(path);
        }
    }
}
