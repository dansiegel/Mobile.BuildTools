using System.Collections.Generic;
using System.IO;

namespace Mobile.BuildTools.Configuration
{
    internal interface IPlatformConfigManager
    {
        bool ResourceExists(string name, out string path);
        StreamReader GetStreamReader(string name);
        IEnumerable<string> GetEnvironments();
    }
}
