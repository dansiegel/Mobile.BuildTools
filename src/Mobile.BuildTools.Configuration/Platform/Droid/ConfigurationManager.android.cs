using Android.App;
using Android.Content;

namespace Mobile.BuildTools.Configuration
{
    public partial class ConfigurationManager
    {
        /// <summary>
        /// Initializes the <see cref="ConfigurationManager.Current"/> instance without
        /// enabling Runtime Environments using the <see cref="Application.Context"/>.
        /// </summary>
        /// <returns>The <see cref="IConfigurationManager"/>.</returns>
        public static IConfigurationManager Init() =>
            Init(false, Application.Context);

        /// <summary>
        /// Initializes the <see cref="ConfigurationManager.Current"/> instance without
        /// enabling Runtime Environments using the specified <see cref="Context"/>.
        /// </summary>
        /// <param name="context">The specified <see cref="Context"/>.</param>
        /// <returns>The <see cref="IConfigurationManager"/>.</returns>
        public static IConfigurationManager Init(Context context) =>
            Init(false, context);

        /// <summary>
        /// Initializes the <see cref="ConfigurationManager.Current"/> instance with
        /// Runtime Environments enbled if <paramref name="enableRuntimeEnvironments"/>
        /// is <see langword="true" /> using the <see cref="Application.Context"/>.
        /// </summary>
        /// <param name="enableRuntimeEnvironments">Enables Runtime Environmens when <see langword="true" />.</param>
        /// <returns>The <see cref="IConfigurationManager"/>.</returns>
        public static IConfigurationManager Init(bool enableRuntimeEnvironments) =>
            Init(enableRuntimeEnvironments, Application.Context);

        /// <summary>
        /// Initializes the <see cref="ConfigurationManager.Current"/> instance with
        /// Runtime Environments enbled if <paramref name="enableRuntimeEnvironments"/>
        /// is <see langword="true" /> using the specified <see cref="Context"/>.
        /// </summary>
        /// <param name="enableRuntimeEnvironments">Enables Runtime Environmens when <see langword="true" />.</param>
        /// <param name="context">The specified <see cref="Context"/>.</param>
        /// <returns>The <see cref="IConfigurationManager"/>.</returns>
        public static IConfigurationManager Init(bool enableRuntimeEnvironments, Context context) =>
            InitInternal(enableRuntimeEnvironments, new AndroidConfigManager(context ?? Application.Context));
    }
}
