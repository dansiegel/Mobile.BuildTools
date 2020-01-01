using System.IO;
using System.Linq;
using Foundation;

[assembly: LinkerSafe]
namespace Mobile.BuildTools.Configuration
{
    public partial class ConfigurationManager
    {
        private const string DEFAULT_CONFIG_FILENAME = "app.config";
        private static string _currentConfigName = DEFAULT_CONFIG_FILENAME;

        public static void Init(string config = DEFAULT_CONFIG_FILENAME) => Update(config);

        public static void Init(string environmentConfig, string config = DEFAULT_CONFIG_FILENAME)
        {
            _currentConfigName = config;
            using (var configStream = GetStreamReader(config))
            using (var environmentStream = GetStreamReader(environmentConfig))
            {
                var xDocument = TransformationHelper.Transform(configStream.ReadToEnd(), environmentStream.ReadToEnd());
                InitInternal(xDocument);
            }

            var files = Directory.EnumerateFiles("Assets", "app.*.config");
            Environments = files.Select(x => x.Split('.')[1]).ToList();
        }

        private static StreamReader GetStreamReader(string config) =>
            new StreamReader(Path.Combine("Assets", config));
    }
}