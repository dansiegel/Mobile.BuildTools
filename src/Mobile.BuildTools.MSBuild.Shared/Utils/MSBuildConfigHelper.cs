using Microsoft.Build.Framework;
using Mobile.BuildTools.Models;

namespace Mobile.BuildTools.Utils
{
    public static partial class MSBuildConfigHelper
    {
        public static BuildToolsConfig GetConfig(ITaskItem item) =>
            ConfigHelper.GetConfig(item.ItemSpec);
    }
}
