using System.Collections.Generic;
using System.Collections.ObjectModel;

#pragma warning disable IDE0040 // Add accessibility modifiers
#pragma warning disable IDE1006 // Naming Styles
namespace Mobile.BuildTools.Configuration
{
    public interface IConfigurationManager
    {
        INameValueCollection AppSettings { get; }
        ConnectionStringCollection ConnectionStrings { get; }

        bool EnvironmentExists(string name);
        IReadOnlyList<string> Environments { get; }
        void Reset();
        void Transform(string name);
    }
}
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore IDE0040 // Add accessibility modifiers
