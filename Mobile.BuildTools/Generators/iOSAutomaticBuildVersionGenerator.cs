using System.IO;
using System.Xml.Linq;

namespace Mobile.BuildTools.Generators
{
    public class iOSAutomaticBuildVersionGenerator : BuildVersionGeneratorBase
    {
        protected override void ProcessManifest(string plistPath, string buildNumber)
        {
            var infoPlist = XElement.Parse(File.ReadAllText(plistPath));
            var dict = GetDictionary(infoPlist);
            WalkDictionary(dict, buildNumber);
            File.WriteAllText(plistPath, infoPlist.ToString());
        }

        private XElement GetDictionary(XElement plist)
        {
            if (plist.FirstNode is XElement element && element.Name.LocalName == "dict")
            {
                return element;
            }

            return null;
        }

        private void WalkDictionary(XElement dict, string buildNumber)
        {
            var currentNode = dict.FirstNode as XElement;
            while (currentNode != null)
            {
                var valueNode = currentNode.NextNode as XElement;
                switch (currentNode.Value)
                {
                    case "CFBundleVersion":
                        // CFBundleVersion
                        // $(BUILD_ID)
                        Log.LogMessage($"Setting the CFBundleVersion to: '{buildNumber}'");
                        valueNode.Value = buildNumber;
                        break;
                    case "CFBundleShortVersionString":
                        // CFBundleShortVersionString
                        // 1.0.$(BUILD_ID)
                        var shortVersion = $"{SanitizeVersion(valueNode.Value)}.{buildNumber}";
                        Log.LogMessage($"Setting the CFBundleShortVersionString to: '{shortVersion}'");
                        valueNode.Value = shortVersion;
                        break;
                }

                currentNode = GetNextKey(currentNode);
            }
        }

        private XElement GetNextKey(XNode node)
        {
            if (!(node.NextNode is XElement next)) return null;

            if (next.Name.LocalName == "key")
                return next;

            return GetNextKey(next);
        }
    }
}
