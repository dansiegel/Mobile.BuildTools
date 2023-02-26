#nullable enable
namespace Mobile.BuildTools.Configuration;

/// <summary>
/// Provides a foundation for containing a Connection String
/// </summary>
/// <param name="Name">The name or key of the ConnectionString.</param>
/// <param name="ProviderName">The provider type name. This is typically ignored.</param>
/// <param name="ConnectionString">The connection string.</param>
public record ConnectionStringSettings(string Name, string? ProviderName, string ConnectionString);
