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
        public void AppManifestTargetsUseTopLevelConditionalImports()
        {
            var document = LoadTargets("Mobile.BuildTools.AppManifests.targets");
            var text = document.ToString();

            Assert.DoesNotContain("<When", text);
            Assert.Contains("<Import Project=\"AndroidManifest.targets\"", text);
            Assert.Contains("<Import Project=\"AppleManifests.targets\"", text);
            Assert.Contains("GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'", text);
            Assert.Contains("GetTargetPlatformIdentifier('$(TargetFramework)')) == 'ios'", text);
            Assert.Contains("GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst'", text);
        }

        [Fact]
        public void AppManifestTargetsLoadTasksFromAppManifestsAssembly()
        {
            var text = LoadTargets("Mobile.BuildTools.AppManifests.targets").ToString();

            Assert.Contains("Mobile.BuildTools.AppManifests.dll", text);
            Assert.DoesNotContain("Mobile.BuildTools.dll", text);
        }

        [Fact]
        public void AppleTargetsCopyDetectedManifestAndReplaceConsumedManifestItems()
        {
            var text = LoadTargets("AppleManifests.targets").ToString();

            Assert.Contains("<_MBTSourcePlist Condition=\" '$(AppBundleManifest)' != '' \">$(AppBundleManifest)</_MBTSourcePlist>", text);
            Assert.Contains("<_MBTSourcePlist Condition=\" '$(_MBTSourcePlist)' == '' \">$(_AppManifest)</_MBTSourcePlist>", text);
            Assert.Contains("<__MBTInputManifest Include=\"$(_MBTSourcePlist)\"", text);
            Assert.Contains("<AppBundleManifest>$(_MBTUpdatedManifest)</AppBundleManifest>", text);
            Assert.Contains("<PartialAppManifest Remove=\"@(PartialAppManifest)\"", text);
            Assert.Contains("<PartialAppManifest Include=\"$(_MBTUpdatedManifest)\"", text);
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
