using System.Collections;

namespace Mobile.BuildTools.Configuration
{
    /// <summary>
    /// Contains what's needed for a specific collection of Connection Strings.
    /// </summary>
    public class ConnectionStringCollection : IEnumerable<ConnectionStringSettings>
    {
        private List<ConnectionStringSettings> _settings { get; }

        /// <summary>
        /// Initializes a new <see cref="ConnectionStringCollection"/> with a specified
        /// <see cref="IEnumerable{ConnectionStringSettings}"/>.
        /// </summary>
        /// <param name="settings"></param>
        public ConnectionStringCollection(IEnumerable<ConnectionStringSettings> settings)
        {
            _settings = new List<ConnectionStringSettings>();
            foreach(var setting in settings)
            {
                if (!_settings.Any(x => x.Name == setting.Name))
                    _settings.Add(setting);
            }
        }

        /// <summary>
        /// Gets the Keys for the included <see cref="ConnectionStringSettings"/>.
        /// </summary>
        public IEnumerable<string> Keys => _settings.Select(x => x.Name).Distinct();

        /// <summary>
        /// Gets a specified <see cref="ConnectionStringSettings"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConnectionStringSettings this[string name] =>
            _settings.FirstOrDefault(x => x.Name == name);

        /// <summary>
        /// Gets the ConnectionString for a specified key
        /// </summary>
        /// <param name="name">The name of key of the Connection String</param>
        /// <returns>The Connection String.</returns>
        public string GetConnectionString(string name) =>
            this[name].ConnectionString;

        /// <summary>
        /// Gets the <see cref="IEnumerator{ConnectionStringSettings}"/>.
        /// </summary>
        /// <returns>The <see cref="IEnumerator{ConnectionStringSettings}"/>.</returns>
        public IEnumerator<ConnectionStringSettings> GetEnumerator() =>
            _settings.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
