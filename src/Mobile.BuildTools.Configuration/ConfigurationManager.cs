using System.Collections.Generic;

namespace Mobile.BuildTools.Configuration
{
    public partial class ConfigurationManager 
    {
        private static IConfigurationManager _current;
        public static IConfigurationManager Current
        {
            get
            {
                if (_current is null)
                    Init(false);

                return _current;
            }
        }

        public static INameValueCollection AppSettings => Current.AppSettings;

        public static ConnectionStringCollection ConnectionStrings => Current.ConnectionStrings;

        public static IReadOnlyList<string> Environments => Current.Environments;

        public static bool EnvironmentExists(string name) => Current.EnvironmentExists(name);

        public static void Reset() => Current.Reset();

        public static void Transform(string name) => Current.Transform(name);

        private static IConfigurationManager InitInternal(bool enableEnvironments, IPlatformConfigManager platformConfig)
        {
            var configManager = new ConfigurationManagerImplementation(enableEnvironments, platformConfig);
            return _current = configManager;
        }
    }
}
