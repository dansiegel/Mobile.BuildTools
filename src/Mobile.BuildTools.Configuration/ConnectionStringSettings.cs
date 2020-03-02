namespace Mobile.BuildTools.Configuration
{
    public struct ConnectionStringSettings
    {
        public ConnectionStringSettings(string name, string providerName, string connectionString)
        {
            Name = name;
            ProviderName = providerName;
            ConnectionString = connectionString;
        }

        public string Name { get; }
        public string ProviderName { get; }
        public string ConnectionString { get; }

        public override bool Equals(object obj)
        {
            if(obj is ConnectionStringSettings settings)
            {
                return settings.Name == Name && settings.ConnectionString == ConnectionString;
            }

            return false;
        }

        public override int GetHashCode() => $"{Name}-{ConnectionString}".GetHashCode();

        public static bool operator ==(ConnectionStringSettings left, ConnectionStringSettings right) =>
            left.Equals(right);

        public static bool operator !=(ConnectionStringSettings left, ConnectionStringSettings right) =>
            !(left == right);
    }
}
