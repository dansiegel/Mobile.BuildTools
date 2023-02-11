using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal interface IXmlOriginalDocumentService
{
    XmlNodeList SelectNodes(string path, XmlNamespaceManager nsmgr);
}
