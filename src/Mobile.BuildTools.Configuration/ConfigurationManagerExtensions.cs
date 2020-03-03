using System.Collections.Generic;

namespace Mobile.BuildTools.Configuration
{
    internal static class ConfigurationManagerExtensions
    {
        public static ConnectionStringCollection ToConnectionStringCollection(this IEnumerable<ConnectionStringSettings> settings) =>
            new ConnectionStringCollection(settings);

        public static NameValueCollection ToNameValueCollection(this IList<KeyValuePair<string, string>> keyPairs)
        {
            return new NameValueCollection(keyPairs);
        }
    }
}
