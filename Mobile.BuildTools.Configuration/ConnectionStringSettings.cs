#pragma warning disable IDE0040 // Add accessibility modifiers
namespace Mobile.BuildTools.Configuration
{
    public struct ConnectionStringSettings
    {
        internal ConnectionStringSettings(string name, string providerName, string connectionString)
        {
            Name = name;
            ProviderName = providerName;
            ConnectionString = connectionString;
        }

        public string Name { get; }
        public string ProviderName { get; }
        public string ConnectionString { get; }
    }
}
#pragma warning restore IDE0040 // Add accessibility modifiers
