using Android.App;
using Android.Content;

namespace Mobile.BuildTools.Configuration
{
    public partial class ConfigurationManager
    {
        public static void Init() => Init(false, null);

        public static void Init(Context context) => Init(false, context);

        public static void Init(bool enableRuntimeEnvironments) => Init(enableRuntimeEnvironments, null);

        public static void Init(bool enableRuntimeEnvironments, Context context) =>
            InitInternal(enableRuntimeEnvironments, new AndroidConfigManager(context ?? Application.Context));
    }
}
