using System.IO;
using System.Linq;
using Tizen.Applications;

namespace Mobile.BuildTools.Configuration
{
    public partial class ConfigurationManager
    {
        private const string DEFAULT_CONFIG_FILENAME = "app.config";
        private static string _currentConfigName = DEFAULT_CONFIG_FILENAME;

        public static void Init()
        {
            Update(DEFAULT_CONFIG_FILENAME);
        }

        private static StreamReader GetStreamReader(string config)
        {
            var path = Directory.GetFiles(Application.Current.DirectoryInfo.Resource, config, SearchOption.AllDirectories).FirstOrDefault();

            if (string.IsNullOrEmpty(path))
                new StreamReader(Stream.Null);

            return new StreamReader(path);
        }
    }
}
