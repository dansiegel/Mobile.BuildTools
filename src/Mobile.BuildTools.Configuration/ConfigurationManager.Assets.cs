using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Mobile.BuildTools.Configuration;

public partial class ConfigurationManager
{
    /// <summary>
    /// Initializes configuration from an asynchronous package-asset reader.
    /// The reader is called with app.config and its returned stream is disposed.
    /// Use a framework package API (for example MAUI FileSystem or Uno StorageFile),
    /// especially on WebAssembly where package files require asynchronous access.
    /// </summary>
    /// <remarks>
    /// Loads the final transformed configuration once. Runtime environment transforms
    /// are disabled. This keeps transport separate from parsing and avoids blocking
    /// browser threads, reflection-based platform discovery or embedded config values.
    /// Reset re-reads the captured bytes; call InitAsync again to fetch another asset.
    /// Current is updated only after loading and parsing have completed successfully.
    /// </remarks>
    public static async Task<IConfigurationManager> InitAsync(
        Func<string, CancellationToken, Task<Stream>> openPackageAsset,
        CancellationToken cancellationToken = default)
    {
        if (openPackageAsset is null) throw new ArgumentNullException(nameof(openPackageAsset));
        cancellationToken.ThrowIfCancellationRequested();
        using var source = await openPackageAsset("app.config", cancellationToken).ConfigureAwait(false);
        if (source is null) throw new InvalidOperationException("The package asset reader returned no app.config stream.");
        using var content = new MemoryStream();
        await source.CopyToAsync(content, 81920, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        return InitInternal(false, new PackageAssetConfigManager(content.ToArray()));
    }

    private sealed class PackageAssetConfigManager : IPlatformConfigManager
    {
        private readonly byte[] _content;

        public PackageAssetConfigManager(byte[] content) => _content = content;

        public IEnumerable<string> GetEnvironments() => Array.Empty<string>();

        public StreamReader GetStreamReader(string name)
        {
            if (!ResourceExists(name, out _)) throw new FileNotFoundException("Configuration asset not found.", name);
            return new StreamReader(new MemoryStream(_content, writable: false));
        }

        public bool ResourceExists(string name, out string path)
        {
            path = name == "app.config" || name == "app" ? "app.config" : null;
            return path != null;
        }
    }
}
