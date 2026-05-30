using System;
using System.IO;
using System.Xml.Linq;
using Xunit;

namespace Mobile.BuildTools.Tests.Fixtures.Generators
{
    public class AppManifestTargetsFixture
    {
        [Fact]
        public void AndroidTargetsRedirectBuildToProcessedManifest()
        {
            var text = LoadTargets("AndroidManifest.targets").ToString();

            Assert.Contains("<_AndroidManifestAbs>$(_MBTUpdatedManifest)</_AndroidManifestAbs>", text);
            Assert.Contains("ManifestPath=\"$(_MBTUpdatedManifest)\"", text);
            Assert.Contains("OutputManifestPath=\"$(_MBTUpdatedManifest)\"", text);
            Assert.Contains("<FileWrites Include=\"$(_MBTUpdatedManifest)\"", text);
        }

        [Fact]
        public void AppleTargetsCopyDetectedManifestAndReplacePartialManifestItem()
        {
            var text = LoadTargets("AppleManifests.targets").ToString();

            Assert.Contains("<_MBTSourcePlist Condition=\" '$(AppBundleManifest)' != '' \">$(AppBundleManifest)</_MBTSourcePlist>", text);
            Assert.Contains("<_MBTSourcePlist Condition=\" '$(_MBTSourcePlist)' == '' \">$(_AppManifest)</_MBTSourcePlist>", text);
            Assert.Contains("<__MBTInputManifest Include=\"$(_MBTSourcePlist)\"", text);
            Assert.Contains("<_AppManifest>$(_MBTUpdatedManifest)</_AppManifest>", text);
            Assert.Contains("<_PartialAppManifest Remove=\"@(_PartialAppManifest)\"", text);
            Assert.Contains("<_PartialAppManifest Include=\"$(_MBTUpdatedManifest)\"", text);
        }

        private static XDocument LoadTargets(string fileName)
        {
            var path = Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "..",
                "src",
                "Mobile.BuildTools.AppManifests",
                fileName));
            return XDocument.Load(path);
        }
    }
}
