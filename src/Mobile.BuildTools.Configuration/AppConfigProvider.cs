using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

#nullable enable
namespace Mobile.BuildTools.Configuration;

internal class AppConfigProvider : IConfigurationProvider
{
    private IConfigurationManager _manager { get; }
    private ConfigurationReloadToken _reloadToken = new ConfigurationReloadToken();

    public AppConfigProvider(IConfigurationManager manager)
    {
        _manager = manager;
        Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        if (_manager.Environments.Count > 1)
            _manager.SettingsChanged += (s, e) =>
            {
                Load();
                OnReload();
            };
    }

    private IDictionary<string, string?> Data { get; set; }

    /// <summary>
    /// Attempts to find a value with the given key, returns true if one is found, false otherwise.
    /// </summary>
    /// <param name="key">The key to lookup.</param>
    /// <param name="value">The value found at key if one is found.</param>
    /// <returns>True if key has a value, false otherwise.</returns>
    public virtual bool TryGet(string key, out string? value)
        => Data.TryGetValue(key, out value);

    /// <summary>
    /// Sets a value for a given key.
    /// </summary>
    /// <param name="key">The configuration key to set.</param>
    /// <param name="value">The value to set.</param>
    public virtual void Set(string key, string? value)
    {
        // TODO: Evaluate if this should throw an exception instead
    }

    /// <summary>
    /// Returns a <see cref="IChangeToken"/> that can be used to listen when this provider is reloaded.
    /// </summary>
    /// <returns>The <see cref="IChangeToken"/>.</returns>
    public IChangeToken GetReloadToken()
    {
        return _reloadToken;
    }

    public void Load()
    {
        Data = _manager.ToData();
    }

    /// <summary>
    /// Returns the list of keys that this provider has.
    /// </summary>
    /// <param name="earlierKeys">The earlier keys that other providers contain.</param>
    /// <param name="parentPath">The path for the parent IConfiguration.</param>
    /// <returns>The list of keys for this provider.</returns>
    public virtual IEnumerable<string> GetChildKeys(
        IEnumerable<string> earlierKeys,
        string? parentPath)
    {
        var results = new List<string>();

        if (parentPath is null)
        {
            foreach (KeyValuePair<string, string?> kv in Data)
            {
                results.Add(Segment(kv.Key, 0));
            }
        }
        else
        {
            Debug.Assert(ConfigurationPath.KeyDelimiter == ":");

            foreach (KeyValuePair<string, string?> kv in Data)
            {
                if (kv.Key.Length > parentPath.Length &&
                    kv.Key.StartsWith(parentPath, StringComparison.OrdinalIgnoreCase) &&
                    kv.Key[parentPath.Length] == ':')
                {
                    results.Add(Segment(kv.Key, parentPath.Length + 1));
                }
            }
        }

        results.AddRange(earlierKeys);

        results.Sort(ConfigurationKeyComparer.Comparison);

        return results;
    }

    private static string Segment(string key, int prefixLength)
    {
        var indexOf = key.IndexOf(ConfigurationPath.KeyDelimiter, prefixLength, StringComparison.OrdinalIgnoreCase);
        return indexOf < 0 ? key.Substring(prefixLength) : key.Substring(prefixLength, indexOf - prefixLength);
    }

    /// <summary>
    /// Triggers the reload change token and creates a new one.
    /// </summary>
    protected void OnReload()
    {
        var previousToken = Interlocked.Exchange(ref _reloadToken, new ConfigurationReloadToken());
        previousToken.OnReload();
    }

    /// <summary>
    /// Generates a string representing this provider name and relevant details.
    /// </summary>
    /// <returns> The configuration name. </returns>
    public override string ToString() => $"{GetType().Name}";
}
