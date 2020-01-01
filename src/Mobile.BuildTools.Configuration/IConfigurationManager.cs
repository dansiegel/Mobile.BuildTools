using System.Collections.Generic;
using System.Collections.ObjectModel;

#pragma warning disable IDE0040 // Add accessibility modifiers
#pragma warning disable IDE1006 // Naming Styles
namespace Mobile.BuildTools.Configuration
{
    internal interface IConfigurationManager
    {
        NameValueCollection AppSettings { get; }
        ReadOnlyDictionary<string, ConnectionStringSettings> ConnectionStrings { get; }

        bool EnvironmentExists(string name);
        IReadOnlyList<string> Environments { get; }
    }
}
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore IDE0040 // Add accessibility modifiers
