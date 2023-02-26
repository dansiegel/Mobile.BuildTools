namespace Mobile.BuildTools.Configuration
{
    public partial class ConfigurationManager
    {
        /// <summary>
        /// Initializes the <see cref="ConfigurationManager.Current"/> instance without
        /// enabling Runtime Environments.
        /// </summary>
        /// <returns>The <see cref="IConfigurationManager"/>.</returns>
        public static IConfigurationManager Init() =>
            Init(false);

        /// <summary>
        /// Initializes the <see cref="ConfigurationManager.Current"/> instance with
        /// Runtime Environments enbled if <paramref name="enableRuntimeEnvironments"/>
        /// is <see langword="true" />.
        /// </summary>
        /// <param name="enableRuntimeEnvironments">Enables Runtime Environmens when <see langword="true" />.</param>
        /// <returns>The <see cref="IConfigurationManager"/>.</returns>
        public static IConfigurationManager Init(bool enableRuntimeEnvironments) =>
            InitInternal(enableRuntimeEnvironments, new AppleConfigManager());
    }
}
