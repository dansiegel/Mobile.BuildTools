using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

#pragma warning disable IDE0040 // Add accessibility modifiers
#pragma warning disable IDE1006 // Naming Styles
namespace Mobile.BuildTools.Configuration
{
    public partial class ConfigurationManager : IConfigurationManager
    {
        // ReSharper disable InconsistentNaming
        private const string APP_SETTINGS = "appSettings";
        private const string CONNECTION_STRING = "connectionString";
        private const string CONNECTION_STRINGS = "connectionStrings";
        private const string DEFAULT_CONFIG_FILENAME = "app.config";
        private const string KEY = "key";
        private const string NAME = "name";
        private const string PROVIDER_NAME = "providerName";

        private const string VALUE = "value";
        // ReSharper enable InconsistentNaming

        private static IConfigurationManager _current;
        internal static IConfigurationManager Current
        {
            get
            {
                if(_current is null)
                {
                    Init();
                }

                return _current;
            }
        }

        public static NameValueCollection AppSettings => Current.AppSettings;

        public static ReadOnlyDictionary<string, ConnectionStringSettings> ConnectionStrings => Current.ConnectionStrings;


        private NameValueCollection _appSettings { get; }
        private ReadOnlyDictionary<string, ConnectionStringSettings> _connectionStrings { get; }

        private ConfigurationManager(NameValueCollection appSettings, ReadOnlyDictionary<string, ConnectionStringSettings> connectionStrings)
        {
            _appSettings = appSettings;
            _connectionStrings = connectionStrings;
        }

        NameValueCollection IConfigurationManager.AppSettings => _appSettings;
        ReadOnlyDictionary<string, ConnectionStringSettings> IConfigurationManager.ConnectionStrings => _connectionStrings;

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
            var xNodes = xDocument.Nodes();
            if (xNodes.Count() <= 1) return;
            var appSettings = xDocument.Descendants()
                .Where(t => t.Name == APP_SETTINGS)
                .Elements()
                .ToList()
                .Select(GenerateKeyValueFromItem)
                .Where(i => !string.IsNullOrWhiteSpace(i.Key))
                .ToList();

            var connectionStrings = xDocument.Descendants()
                .Where(t => t.Name == CONNECTION_STRINGS)
                .Elements()
                .ToDictionary(xElement => xElement.Attribute(KEY)?.Value.ToString(), GenerateConnetionStringSettingsFromItem);

            _current = new ConfigurationManager(new NameValueCollection(appSettings), new ReadOnlyDictionary<string, ConnectionStringSettings>(connectionStrings));
        }

        private static XDocument TransformXDocument(string inputXmlFile, string xslFile)
        {
            try
            {
                var inputXml = XDocument.Parse(inputXmlFile);
                var xslt = new XslCompiledTransform();
                var sb = new StringBuilder();
                using (var writer = XmlWriter.Create(sb))
                {
                    xslt.Load(xslFile);
                    xslt.Transform(inputXml.CreateReader(ReaderOptions.None), writer);
                    writer.Close();
                    writer.Flush();
                }

                return XDocument.Parse(sb.ToString());
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                //handle the exception your way
                return new XDocument();
            }
        }

        private static ConnectionStringSettings GenerateConnetionStringSettingsFromItem(XElement xElement) => new ConnectionStringSettings(xElement.Attribute(NAME)?.Value,
            xElement.Attribute(PROVIDER_NAME)?.Value,
            xElement.Attribute(CONNECTION_STRING)?.Value);

        private static KeyValuePair<string, string> GenerateKeyValueFromItem(XElement item) => new KeyValuePair<string, string>(
            item?.Attribute(KEY)?.Value,
            item?.Attribute(VALUE)?.Value);

        protected static void InitInternal(List<KeyValuePair<string, string>> keys, Dictionary<string, ConnectionStringSettings> conStr)
        {
            var appSettings = new NameValueCollection(keys);
            var connectionStrings = conStr != null
                ? new ReadOnlyDictionary<string, ConnectionStringSettings>(conStr)
                : new ReadOnlyDictionary<string, ConnectionStringSettings>(new Dictionary<string, ConnectionStringSettings>());

            _current = new ConfigurationManager(appSettings, connectionStrings);
        }
    }
}
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore IDE0040 // Add accessibility modifiers
