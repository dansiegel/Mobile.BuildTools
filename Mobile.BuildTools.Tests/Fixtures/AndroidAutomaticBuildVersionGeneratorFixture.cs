using System.IO;
using System.Xml;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Tests.Mocks;
using Mobile.BuildTools.Versioning;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures
{
    public class AndroidAutomaticBuildVersionGeneratorFixture
    {
        private static readonly string TemplateAndroidManifestPath = @"Templates/MockAndroidManifest.xml";
        private static readonly string TemplateAndroidManifestOutputPath = @"Properties/AndroidManifest.xml";

        private ITestOutputHelper _testOutputHelper { get; }

        public AndroidAutomaticBuildVersionGeneratorFixture(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            Directory.CreateDirectory("Properties");
        }

        private AndroidAutomaticBuildVersionGenerator CreateGenerator(Behavior behavior = Behavior.Timestamp)
        {
            if (File.Exists(TemplateAndroidManifestOutputPath))
                File.Delete(TemplateAndroidManifestOutputPath);

            File.Copy(TemplateAndroidManifestPath, TemplateAndroidManifestOutputPath);

            return new AndroidAutomaticBuildVersionGenerator
            {
                DebugOutput = true,
                Log = new XunitLog(_testOutputHelper),
                ProjectPath = ".",
                VersionOffset = 0,
                Behavior = behavior,
                VersionEnvironment = VersionEnvironment.All
            };
        }

        [Fact]
        public void VersioningDoesNotCorruptManifest()
        {
            var generator = CreateGenerator();
            generator.Execute();

            var ex = Record.Exception(() =>
            {
                var doc = new XmlDocument();
                doc.LoadXml(File.ReadAllText(TemplateAndroidManifestOutputPath));
            });

            Assert.Null(ex);
        }

        [Fact]
        public void VersionCode_SetToBuildNumber()
        {
            var generator = CreateGenerator();
            generator.Execute();

            Assert.Contains($"android:versionCode=\"{generator.BuildNumber}\"", File.ReadAllText(TemplateAndroidManifestOutputPath));
        }

        [Fact]
        public void VersionName_UsesBuildNumber()
        {
            var generator = CreateGenerator();
            generator.Execute();

            Assert.Contains($"android:versionName=\"1.0.{generator.BuildNumber}\"", File.ReadAllText(TemplateAndroidManifestOutputPath));
        }
    }
}
