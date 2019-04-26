using System.IO;
using Foundation;

[assembly: LinkerSafe]
namespace Mobile.BuildTools.Configuration
{
    public partial class ConfigurationManager
    {
        private static string _currentConfigName = DEFAULT_CONFIG_FILENAME;

        public static void Init(string config = DEFAULT_CONFIG_FILENAME)
        {
            Update(config);
        }

        public static void Init(string environmentConfig, string config = DEFAULT_CONFIG_FILENAME)
        {
            _currentConfigName = config;
            using (var configStream = GetStreamReader(config))
            using (var environmentStream = GetStreamReader(environmentConfig))
            {
                var xDocument = TransformXDocument(configStream.ReadToEnd(), environmentStream.ReadToEnd());
                InitInternal(xDocument);
            }
        }

        public static void TransformForEnvironment(string environmentName)
        {
            var fileName = Path.GetFileNameWithoutExtension(_currentConfigName);
            using (var configStream = GetStreamReader(_currentConfigName))
            using (var environmentStream = GetStreamReader($"{fileName}.{environmentName}.config"))
            {
                var xDocument = TransformXDocument(configStream.ReadToEnd(), environmentStream.ReadToEnd());
                InitInternal(xDocument);
            }
        }

        public static void Update(string config = DEFAULT_CONFIG_FILENAME)
        {
            _currentConfigName = config;
            using (var stream = GetStreamReader(config))
                Init(stream);
        }

        private static StreamReader GetStreamReader(string config) =>
            new StreamReader(Path.Combine("Assets", config));
    }
}