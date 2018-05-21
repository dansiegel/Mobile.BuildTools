using System.Collections.Generic;

namespace E2E.Core
{
    public static class CommonLib
    {
        public static IEnumerable<string> GetManifestResourceNames() =>
            typeof(CommonLib).Assembly.GetManifestResourceNames();
    }
}