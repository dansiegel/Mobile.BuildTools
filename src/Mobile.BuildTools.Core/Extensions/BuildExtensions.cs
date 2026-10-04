using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;

namespace Mobile.BuildTools.Build;

internal static class BuildExtensions
{
    public static IDictionary<string, string> GetGlobalProperties(this IBuildEngine buildEngine) =>
        buildEngine is IBuildEngine6 engine
            ? engine.GetGlobalProperties().ToDictionary(property => property.Key, property => property.Value)
            : new Dictionary<string, string>();
}
