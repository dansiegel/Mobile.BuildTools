using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Mobile.BuildTools.Configuration
{
    internal class ConfigurationManagerImplementation : IConfigurationManager
    {
        private const string DefaultAppConfigName = "app.config";
        private IPlatformConfigManager _platformConfig { get; }
        private bool _environmentsEnabled = false;

        public ConfigurationManagerImplementation(bool enableEnvironments, IPlatformConfigManager platformConfig)
        {
            _environmentsEnabled = enableEnvironments;
            _platformConfig = platformConfig;
            Environments = new List<string>(_platformConfig.GetEnvironments());
            Reset();
        }

        public INameValueCollection AppSettings { get; private set; }
        public ConnectionStringCollection ConnectionStrings { get; private set; }
        public IReadOnlyList<string> Environments { get; internal set; }

        public bool EnvironmentExists(string name) =>
            EnvironmentExists(name, out var _);

        private bool EnvironmentExists(string name, out string environmentName)
        {
            if (!_environmentsEnabled)
            {
                environmentName = null;
                return false;
            }

            environmentName = Environments.FirstOrDefault(x => x.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            return environmentName != null;
        }

        public void Reset()
        {
            using var streamReader = _platformConfig.GetStreamReader(DefaultAppConfigName);
            using var reader = XmlReader.Create(streamReader);
            var xDocument = XDocument.Load(reader);
            InitInternal(xDocument);
        }

        public void Transform(string name)
        {
            if (!_environmentsEnabled)
                return;

            if(EnvironmentExists(name, out var actualEnvironmentName))
            {
                using var configStream = _platformConfig.GetStreamReader(DefaultAppConfigName);
                using var environmentStream = _platformConfig.GetStreamReader($"app.{actualEnvironmentName}.config");
                var xDocument = TransformationHelper.Transform(configStream.ReadToEnd(), environmentStream.ReadToEnd());
                InitInternal(xDocument);
            }
            else
            {
                Reset();
            }
        }

        protected void InitInternal(XDocument xDocument)
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

                AppSettings = appSettings;
                ConnectionStrings = connectionStrings;
            }
            catch (Exception ex)
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
    }
}
