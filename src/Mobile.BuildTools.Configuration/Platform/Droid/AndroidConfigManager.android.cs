using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.Content;

namespace Mobile.BuildTools.Configuration
{
    internal sealed class AndroidConfigManager : IPlatformConfigManager
    {
        private Context Context { get; }

        public AndroidConfigManager(Context context)
        {
            Context = context;
        }

        private IEnumerable<string> GetConfigFiles() =>
            Context.Assets
                   .List(string.Empty)
                   .Where(x => Path.GetExtension(x).ToLower() == ".config");

        public IEnumerable<string> GetEnvironments()
        {
            var assets = GetConfigFiles().Where(x => !Path.GetFileName(x).Equals("app.config"));
            return assets.Select(x => x.Split('.')[1]).ToList();
        }

        public StreamReader GetStreamReader(string name) =>
            new StreamReader(Context.Assets.Open(name));

        public bool ResourceExists(string name, out string path)
        {
            path = GetConfigFiles().FirstOrDefault(x => Path.GetFileName(x) == name || $"app.{name}.config" == Path.GetFileName(x));
            return !string.IsNullOrEmpty(path);
        }
    }
}