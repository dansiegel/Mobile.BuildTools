using System.ComponentModel;

namespace Mobile.BuildTools.Configuration
{
    /// <summary>
    /// The <see cref="ConfigurationManager"/> class provides a base for working with
    /// the app.config.
    /// </summary>
    public partial class ConfigurationManager 
    {
        private static IConfigurationManager _current;
        /// <summary>
        /// Gets the current instance of the <see cref="IConfigurationManager" />
        /// </summary>
        public static IConfigurationManager Current
        {
            get
            {
                if (_current is null)
                    Init(false);

                return _current;
            }
            [EditorBrowsable(EditorBrowsableState.Never)]
            set => _current = value;
        }

        /// <summary>
        /// Gets the AppSettings values from your app.config
        /// </summary>
        public static INameValueCollection AppSettings => Current.AppSettings;

        /// <summary>
        /// Gets the <see cref="ConnectionStringCollection"/> containing any ConnectionStrings in your configuration.
        /// </summary>
        public static ConnectionStringCollection ConnectionStrings => Current.ConnectionStrings;

        /// <summary>
        /// Gets a ReadOnlyList of Environment Names
        /// </summary>
        public static IReadOnlyList<string> Environments => Current.Environments;

        /// <summary>
        /// Checks whether a specified environment exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool EnvironmentExists(string name) => Current.EnvironmentExists(name);

        /// <summary>
        /// Resets the Current Instance to the default app.config state.
        /// </summary>
        /// <remarks>
        /// You would want to do this if you were trying to change from a specific Environment back to
        /// the default app.config state.
        /// </remarks>
        public static void Reset() => Current.Reset();

        /// <summary>
        /// Transforms the Configuration Manager to use the settings from a specific environment.
        /// </summary>
        /// <param name="name"></param>
        public static void Transform(string name) => Current.Transform(name);

        private static IConfigurationManager InitInternal(bool enableEnvironments, IPlatformConfigManager platformConfig)
        {
            var configManager = new ConfigurationManagerImplementation(enableEnvironments, platformConfig);
            return _current = configManager;
        }
    }
}
