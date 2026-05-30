using System.IO;
using System.Xml;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Generators.Versioning;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures.Generators
{
    public class AutomaticBuildVersionGeneratorFixture : FixtureBase
    {
        private const string TemplateAndroidManifestPath = @"Templates/MockAndroidManifest.xml";
        private const string TemplateAndroidManifestOutputPath = @"Generated/VersionedAndroidManifest.xml";
        private const string TemplateInfoPlistPath = @"Templates/MockInfo.plist";
        private const string TemplateInfoPlistOutputPath = @"Generated/VersionedInfo.plist";

        public AutomaticBuildVersionGeneratorFixture(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [Fact]
        public void AndroidVersioningDoesNotCorruptManifest()
        {
            var generator = new AndroidAutomaticBuildVersionGenerator(GetConfiguration(), TemplateAndroidManifestPath, TemplateAndroidManifestOutputPath);

            ((IGenerator)generator).Execute();

            var generated = File.ReadAllText(TemplateAndroidManifestOutputPath);
            var doc = new XmlDocument();
            doc.LoadXml(generated);
            Assert.Contains($"android:versionCode=\"{generator.BuildNumber}\"", generated);
            Assert.Contains($"android:versionName=\"1.0.{generator.BuildNumber}\"", generated);
        }

        [Fact]
        public void iOSVersioningDoesNotCorruptPlist()
        {
            var generator = new iOSAutomaticBuildVersionGenerator(GetConfiguration(), TemplateInfoPlistPath, TemplateInfoPlistOutputPath);

            ((IGenerator)generator).Execute();

            var generated = File.ReadAllText(TemplateInfoPlistOutputPath);
            var doc = new XmlDocument();
            doc.LoadXml(generated);
            Assert.Contains("<key>CFBundleVersion</key>", generated);
            Assert.Contains($"<string>{generator.BuildNumber}</string>", generated);
            Assert.Contains("<key>CFBundleShortVersionString</key>", generated);
            Assert.Contains($"<string>1.0.{generator.BuildNumber}</string>", generated);
        }
    }
}
