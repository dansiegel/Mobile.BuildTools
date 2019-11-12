using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Web.XmlTransform;

namespace Mobile.BuildTools.Configuration
{
    internal static class TransformationHelper
    {
        public static XDocument Transform(string appConfig, string transformConfig)
        {
            var transformation = new XmlTransformation(transformConfig, false, null);

            var configurationFileDocument = new XmlTransformableDocument();
            configurationFileDocument.LoadXml(appConfig);

            transformation.Apply(configurationFileDocument);

            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb))
            {
                configurationFileDocument.WriteContentTo(writer);
            }

            return XDocument.Parse(sb.ToString());
        }
    }
}
