using System.IO;
using Android;
using Android.App;
using Android.Content;

[assembly: LinkerSafe]
namespace Mobile.BuildTools.Configuration
{
    public partial class ConfigurationManager
    {
        private static string _currentConfigName = DEFAULT_CONFIG_FILENAME;
        private static Context _currentContext;
        private static Context CurrentContext => _currentContext ?? Application.Context;

        public static void Init(string config = "app.config") => Init(Application.Context, config);

        public static void Init(Context context, string config = DEFAULT_CONFIG_FILENAME)
        {
            _currentContext = context;
            Update(config);
        }

        public static void Init(Context context, string environmentConfig, string config = DEFAULT_CONFIG_FILENAME)
        {
            _currentContext = context;
            _currentConfigName = config;

            using (var configStream = new StreamReader(context.Assets.Open(config)))
            using (var environmentStream = new StreamReader(context.Assets.Open(environmentConfig)))
            {
                var xDocument = TransformXDocument(configStream.ReadToEnd(), environmentStream.ReadToEnd());
                InitInternal(xDocument);
            }
        }

        public static void TransformForEnvironment(string environmentName)
        {
            var fileName = Path.GetFileNameWithoutExtension(_currentConfigName);
            using (var configStream = new StreamReader(_currentContext.Assets.Open(_currentConfigName)))
            using (var environmentStream = new StreamReader(_currentContext.Assets.Open($"{fileName}.{environmentName}.config")))
            {
                var xDocument = TransformXDocument(configStream.ReadToEnd(), environmentStream.ReadToEnd());
                InitInternal(xDocument);
            }
        }

        public static void Update(string config = DEFAULT_CONFIG_FILENAME)
        {
            _currentConfigName = config;
            using (var stream = new StreamReader(_currentContext.Assets.Open(config)))
                Init(stream);
        }
    }
}