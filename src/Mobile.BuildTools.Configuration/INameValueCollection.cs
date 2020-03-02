using System.Collections.Generic;

namespace Mobile.BuildTools.Configuration
{
    public interface INameValueCollection : IReadOnlyList<KeyValuePair<string, string>>
    {
        string this[string key] { get; }
        IEnumerable<string> AllKeys { get; }
    }
}
