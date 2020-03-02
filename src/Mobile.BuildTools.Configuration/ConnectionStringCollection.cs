using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mobile.BuildTools.Configuration
{
    public class ConnectionStringCollection : IEnumerable<ConnectionStringSettings>
    {
        private List<ConnectionStringSettings> _settings { get; }

        public ConnectionStringCollection(IEnumerable<ConnectionStringSettings> settings)
        {
            _settings = new List<ConnectionStringSettings>();
            foreach(var setting in settings)
            {
                if (!_settings.Any(x => x.Name == setting.Name))
                    _settings.Add(setting);
            }
        }

        public IEnumerable<string> Keys => _settings.Select(x => x.Name).Distinct();

        public ConnectionStringSettings this[string name] =>
            _settings.FirstOrDefault(x => x.Name == name);

        public string GetConnectionString(string name) =>
            this[name].ConnectionString;

        public IEnumerator<ConnectionStringSettings> GetEnumerator() =>
            _settings.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
