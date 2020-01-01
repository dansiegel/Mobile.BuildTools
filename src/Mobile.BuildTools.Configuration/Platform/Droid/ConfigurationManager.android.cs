using System.IO;
using System.Linq;
using Android;
using Android.App;
using Android.Content;

[assembly: LinkerSafe]
namespace Mobile.BuildTools.Configuration
{
    public partial class ConfigurationManager
    {
        private const string DEFAULT_CONFIG_FILENAME = "app.config";
        private static string _currentConfigName = DEFAULT_CONFIG_FILENAME;
        private static Context _currentContext;
        private static Context CurrentContext => _currentContext ?? Application.Context;

        public static void Init(string config = DEFAULT_CONFIG_FILENAME) => Init(Application.Context, config);

        public static void Init(Context context, string config = DEFAULT_CONFIG_FILENAME)
        {
            _currentContext = context;
            Update(config);
        }

        public static void Init(Context context, string environmentConfig, string config = DEFAULT_CONFIG_FILENAME)
        {
            _currentContext = context;
            _currentConfigName = config;

            using (var configStream = GetStreamReader(config))
            using (var environmentStream = GetStreamReader(environmentConfig))
            {
                var xDocument = TransformationHelper.Transform(configStream.ReadToEnd(), environmentStream.ReadToEnd());
                InitInternal(xDocument);
            }

            var assets = CurrentContext.Assets.List("app.*.config");
            Environments = assets.Select(x => x.Split('.')[1]).ToList();
        }

        private static StreamReader GetStreamReader(string config) =>
            new StreamReader(CurrentContext.Assets.Open(config));
    }
}