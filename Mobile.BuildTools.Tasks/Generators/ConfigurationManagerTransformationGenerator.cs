using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Logging;

namespace Mobile.BuildTools.Tasks.Generators
{
    public class ConfigurationManagerTransformationGenerator : IGenerator
    {
        // app.config
        public string BaseConfigPath { get; set; }

        public string BuildConfiguration { get; set; }

        public ILog Log { get; set; }
        public bool? DebugOutput { get; set; }

        public void Execute()
        {
            var parentDirectory = Directory.GetParent(BaseConfigPath);
            var configFileName = Path.GetFileNameWithoutExtension(BaseConfigPath);
            var transformationConfigPath = Path.Combine(parentDirectory.FullName, $"{configFileName}.{BuildConfiguration}{Path.GetExtension(BaseConfigPath)}");

            var updatedConfig = TransformXDocument(BaseConfigPath, transformationConfigPath);
            if (updatedConfig is null) return;

            var outputFile = Path.Combine(BaseConfigPath, Path.GetFileName(BaseConfigPath));
            updatedConfig.Save(outputFile);

            if(DebugOutput ?? false)
            {
                Log.LogMessage("*************** Transformed app.config ******************************");
                Log.LogMessage(updatedConfig.ToString());
            }
        }

        private XDocument TransformXDocument(string inputXmlFile, string xslFile)
        {
            try
            {
                var inputXml = XDocument.Parse(inputXmlFile);

                if (DebugOutput ?? false)
                {
                    Log.LogMessage("*************** Initial app.config ******************************");
                    Log.LogMessage(inputXml.ToString());
                }

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
                Log.LogErrorFromException(ex);
                return null;
            }
        }
    }
}
