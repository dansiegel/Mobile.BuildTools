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

        public static void Init()
        {
            var assets = Directory.GetFiles(NSBundle.MainBundle.BundlePath, "app.*.config")
                .Where(x => Path.GetExtension(x).ToLower() == ".config" && 
                        !Path.GetFileName(x).Equals("app.config"))
                .Select(x => Path.GetFileName(x));
            Environments = assets.Select(x => x.Split('.')[1]).ToList();
            var debug = Environments;
            Update(DEFAULT_CONFIG_FILENAME);
        }

        public static void Init(string environmentConfig)
        {
            Init();

            if (environmentConfig.Equals(DEFAULT_CONFIG_FILENAME))
                return;

            if (environmentConfig.Split('.').Count() > 1)
            {
                environmentConfig = environmentConfig.Split('.')[1];
            }

            TransformForEnvironment(environmentConfig);

            //using (var configStream = GetStreamReader(config))
            //using (var environmentStream = GetStreamReader(environmentConfig))
            //{
            //    var xDocument = TransformationHelper.Transform(configStream.ReadToEnd(), environmentStream.ReadToEnd());
            //    InitInternal(xDocument);
            //}

            //var files = Directory.EnumerateFiles("Resources", "app.*.config");
            //Environments = files.Select(x => x.Split('.')[1]).ToList();
        }

        private static StreamReader GetStreamReader(string config)
        {
            var path = Directory.GetFiles(NSBundle.MainBundle.BundlePath, config, SearchOption.AllDirectories).FirstOrDefault();

            if (string.IsNullOrEmpty(path))
                new StreamReader(Stream.Null);

            return new StreamReader(path);
        }
    }
}