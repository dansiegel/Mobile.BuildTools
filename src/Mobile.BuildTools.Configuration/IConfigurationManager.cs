using System.Collections.Generic;

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
