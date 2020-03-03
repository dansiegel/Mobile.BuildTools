namespace Mobile.BuildTools.Configuration
{
    public partial class ConfigurationManager
    {
#if NETSTANDARD
        internal
#else
        public
#endif
        static void Init() => Init(false);

#if NETSTANDARD
        internal
#else
        public
#endif
        static void Init(bool enableRuntimeEnvironments)
        {
            InitInternal(enableRuntimeEnvironments, new CommonConfigManager());
        }
    }
}
