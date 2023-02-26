namespace Mobile.BuildTools.Configuration
{
    public partial class ConfigurationManager
    {
        public static void Init() => Init(false);

        public static void Init(bool enableRuntimeEnvironments) =>
            InitInternal(enableRuntimeEnvironments, new WinUIConfigManager());
    }
}
