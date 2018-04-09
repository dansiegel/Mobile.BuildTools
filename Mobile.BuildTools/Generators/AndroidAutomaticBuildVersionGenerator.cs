using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Mobile.BuildTools.Generators
{
    public class AndroidAutomaticBuildVersionGenerator : BuildVersionGeneratorBase
    {
        protected override string GetManifestPath() =>
            Path.Combine(ProjectPath, "Properties", "AndroidManifest.xml");

        protected override void ProcessManifest(string path, string buildNumber)
        {
            var manifest = XElement.Parse(File.ReadAllText(path));
            ProcessAndroidAttributes(manifest.FirstAttribute, buildNumber);
            File.WriteAllText(path, manifest.ToString());
        }

        private void ProcessAndroidAttributes(XAttribute attribute, string buildNumber)
        {
            switch (attribute.Name.LocalName)
            {
                case "versionCode":
                    attribute.Value = buildNumber;
                    Log.LogMessage($"versionCode: {attribute.Value}");
                    break;
                case "versionName":
                    attribute.Value = $"{SanitizeVersion(attribute.Value)}.{buildNumber}";
                    Log.LogMessage($"versionName: {attribute.Value}");
                    break;
            }

            if (attribute.NextAttribute != null)
                ProcessAndroidAttributes(attribute.NextAttribute, buildNumber);
        }
    }
}
