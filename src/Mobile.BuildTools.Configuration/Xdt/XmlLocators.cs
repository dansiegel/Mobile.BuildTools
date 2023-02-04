using System;
using System.Globalization;
using System.Xml;
using Mobile.BuildTools.Configuration.Xdt.Strings;

namespace Microsoft.Web.XmlTransform;

internal sealed class DefaultLocator : Locator
{
    // Uses all the default behavior

    private static DefaultLocator instance = null;
    internal static DefaultLocator Instance {
        get {
            if (instance == null) {
                instance = new DefaultLocator();
            }
            return instance;
        }
    }
}

internal sealed class Match : Locator
{
    protected override string ConstructPredicate() {
        EnsureArguments(1);

        string keyPredicate = null;

        foreach (var key in Arguments) {
            XmlAttribute keyAttribute = CurrentElement.Attributes.GetNamedItem(key) as XmlAttribute;

            if (keyAttribute != null) {
                var keySegment = string.Format(CultureInfo.InvariantCulture, "@{0}='{1}'", keyAttribute.Name, keyAttribute.Value);
                if (keyPredicate == null) {
                    keyPredicate = keySegment;
                }
                else {
                    keyPredicate = string.Concat(keyPredicate, " and ", keySegment);
                }
            }
            else {
                throw new XmlTransformationException(string.Format(CultureInfo.CurrentCulture,Resources.XMLTRANSFORMATION_MatchAttributeDoesNotExist, key));
            }
        }

        return keyPredicate;
    }
}

internal sealed class Condition : Locator
{
    protected override string ConstructPredicate() {
        EnsureArguments(1, 1);

        return Arguments[0];
    }
}

internal sealed class XPath : Locator
{
    protected override string ParentPath {
        get {
            return ConstructPath();
        }
    }

    protected override string ConstructPath() {
        EnsureArguments(1, 1);

        string xpath = Arguments[0];
        if (!xpath.StartsWith("/", StringComparison.Ordinal)) {
            // Relative XPath
            xpath = AppendStep(base.ParentPath, NextStepNodeTest);
            xpath = AppendStep(xpath, Arguments[0]);
            xpath = xpath.Replace("/./", "/");
        }

        return xpath;
    }
}
