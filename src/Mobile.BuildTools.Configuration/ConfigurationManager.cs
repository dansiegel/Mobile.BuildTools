using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Mobile.BuildTools.Configuration
{
    public partial class ConfigurationManager : IConfigurationManager
    {
        private static ConfigurationManager _current;
        public static IConfigurationManager Current
        {
            get
            {
                if (_current is null)
                    Init();

                return _current;
            }
        }

        public static INameValueCollection AppSettings => Current.AppSettings;

        public static ConnectionStringCollection ConnectionStrings => Current.ConnectionStrings;

#if NETSTANDARD
        public static IReadOnlyList<string> Environments => throw new NotSupportedException();
#else
        public static IReadOnlyList<string> Environments { get; private set; }
#endif

        public static bool EnvironmentExists(string name) => EnvironmentExists(name, out var _);
        public static bool EnvironmentExists(string name, out string environmentName)
        {
            environmentName = Environments.FirstOrDefault(x => x.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            return environmentName != null;
        }

        private NameValueCollection _appSettings;
        private ConnectionStringCollection _connectionStrings;

        private ConfigurationManager()
        {
            _appSettings = new NameValueCollection(new List<KeyValuePair<string, string>>());
            _connectionStrings = new ConnectionStringCollection(Array.Empty<ConnectionStringSettings>());
        }

        private ConfigurationManager(NameValueCollection appSettings, ConnectionStringCollection connectionStrings)
        {
            _appSettings = appSettings;
            _connectionStrings = connectionStrings;
        }

        INameValueCollection IConfigurationManager.AppSettings => _appSettings;
        ConnectionStringCollection IConfigurationManager.ConnectionStrings => _connectionStrings;
        IReadOnlyList<string> IConfigurationManager.Environments => Environments;
        bool IConfigurationManager.EnvironmentExists(string name) => EnvironmentExists(name);
        void IConfigurationManager.Reset() => TransformDefault();
        void IConfigurationManager.Transform(string name) => TransformForEnvironment(name);

        public static void TransformForEnvironment(string environmentName)
        {
            if (!EnvironmentExists(environmentName, out var actualEnvironmentName))
                throw new FileNotFoundException();

            var fileName = Path.GetFileNameWithoutExtension(_currentConfigName);
            using var configStream = GetStreamReader(_currentConfigName);
            using var environmentStream = GetStreamReader($"{fileName}.{actualEnvironmentName}.config");
            var xDocument = TransformationHelper.Transform(configStream.ReadToEnd(), environmentStream.ReadToEnd());
            InitInternal(xDocument);
        }

        public static void TransformDefault() => Update(DEFAULT_CONFIG_FILENAME);

        public static void Update(string config)
        {
            _currentConfigName = config;
            using var stream = GetStreamReader(config);
            Init(stream);
        }

        protected static void Init(StreamReader streamReader)
        {
            using (var reader = XmlReader.Create(streamReader))
            {
                var xDocument = XDocument.Load(reader);
                InitInternal(xDocument);
            }
        }

        protected static void InitInternal(XDocument xDocument)
        {
            try
            {
                if (!xDocument.Nodes().Any()) return;

                var appSettings = xDocument.Descendants()
                    .Where(t => t.Name == AppConfigElement.AppSettings)
                    .Elements()
                    .ToList()
                    .Select(GenerateKeyValueFromItem)
                    .Where(i => !string.IsNullOrWhiteSpace(i.Key))
                    .ToList()
                    .ToNameValueCollection();

                var decendents = xDocument.Descendants().Where(t => t.Name == AppConfigElement.ConnectionStrings);
                var elements = decendents.Elements();


                var connectionStrings = xDocument.Descendants()
                        .Where(t => t.Name == AppConfigElement.ConnectionStrings)
                        .Elements()
                        .Select(x => GenerateConnectionStringSettingsFromItem(x))
                        .ToConnectionStringCollection();

                var configManager = _current ?? (_current = new ConfigurationManager());

                configManager._appSettings = appSettings;
                configManager._connectionStrings = connectionStrings;
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        private static ConnectionStringSettings GenerateConnectionStringSettingsFromItem(XElement xElement) => new ConnectionStringSettings(xElement.Attribute(AppConfigElement.Name)?.Value,
            xElement.Attribute(AppConfigElement.ProviderName)?.Value,
            xElement.Attribute(AppConfigElement.ConnectionString)?.Value);

        private static KeyValuePair<string, string> GenerateKeyValueFromItem(XElement item) => new KeyValuePair<string, string>(
            item?.Attribute(AppConfigElement.Key)?.Value,
            item?.Attribute(AppConfigElement.Value)?.Value);

        protected static void InitInternal(List<KeyValuePair<string, string>> keys, IEnumerable<ConnectionStringSettings> conStr)
        {
            var appSettings = new NameValueCollection(keys);
            var connectionStrings = new ConnectionStringCollection(conStr);

            _current = new ConfigurationManager(appSettings, connectionStrings);
        }
    }

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
