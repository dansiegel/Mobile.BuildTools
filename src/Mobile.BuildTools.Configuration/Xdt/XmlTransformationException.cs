using System;

namespace Microsoft.Web.XmlTransform;

// This doesn't do anything, except mark an error as having come from
// the transformation engine
[Serializable]
internal class XmlTransformationException : Exception
{
    public XmlTransformationException(string message)
        : base(message) {
    }

    public XmlTransformationException(string message, Exception innerException)
        : base(message, innerException) {
    }
}
