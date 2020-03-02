using System.IO;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Content.Res;

[assembly: LinkerSafe]
namespace Mobile.BuildTools.Configuration
{
    public partial class ConfigurationManager
    {
        private const string DEFAULT_CONFIG_FILENAME = "app.config";
        private static string _currentConfigName = DEFAULT_CONFIG_FILENAME;
        private static Context _currentContext;
        private static Context CurrentContext => _currentContext ?? Application.Context;

        internal static void Init() => Init(Application.Context, DEFAULT_CONFIG_FILENAME);

        public static void Init(Context context, string config = DEFAULT_CONFIG_FILENAME)
        {
            _currentContext = context;
            _currentConfigName = config;
            var assets = CurrentContext.Assets.List(string.Empty).Where(x => Path.GetExtension(x).ToLower() == ".config" && !Path.GetFileName(x).Equals("app.config"));
            Environments = assets.Select(x => x.Split('.')[1]).ToList();

            Update(config);
        }

        public static void Init(Context context, string environmentConfig, string config = DEFAULT_CONFIG_FILENAME)
        {
            Init(context, config);
            Update(environmentConfig);
        }

        private static StreamReader GetStreamReader(string config)
        {
            return new StreamReader(CurrentContext.Assets.Open(config));
        }
    }
}