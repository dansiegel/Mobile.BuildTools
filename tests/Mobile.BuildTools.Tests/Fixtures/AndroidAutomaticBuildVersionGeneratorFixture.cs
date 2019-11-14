using System.IO;
using System.Xml;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Generators.Versioning;
using Mobile.BuildTools.Models;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0040 // Add accessibility modifiers
namespace Mobile.BuildTools.Tests.Fixtures
{
    public class AndroidAutomaticBuildVersionGeneratorFixture : FixtureBase
    {

        private static readonly string TemplateAndroidManifestPath = @"Templates/MockAndroidManifest.xml";

        private static readonly string TemplateAndroidManifestOutputPath = @"Properties/AndroidManifest.xml";

        public AndroidAutomaticBuildVersionGeneratorFixture(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
            Directory.CreateDirectory("Properties");
        }

        private AndroidAutomaticBuildVersionGenerator CreateGenerator(VersionBehavior behavior = VersionBehavior.Timestamp)
        {
            if (File.Exists(TemplateAndroidManifestOutputPath))
                File.Delete(TemplateAndroidManifestOutputPath);

            File.Copy(TemplateAndroidManifestPath, TemplateAndroidManifestOutputPath);

            return new AndroidAutomaticBuildVersionGenerator(GetConfiguration());
        }

        //[Fact]
        public void VersioningDoesNotCorruptManifest()
        {
            IGenerator generator = CreateGenerator();
            generator.Execute();

            var ex = Record.Exception(() =>
            {
                var doc = new XmlDocument();
                doc.LoadXml(File.ReadAllText(TemplateAndroidManifestOutputPath));
            });

            Assert.Null(ex);
        }

        //[Fact]
        public void VersionCode_SetToBuildNumber()
        {
            IGenerator generator = CreateGenerator();
            generator.Execute();

            //Assert.Contains($"android:versionCode=\"{generator.BuildNumber}\"", File.ReadAllText(generator.ManifestPath));
        }

        //[Fact]
        public void VersionName_UsesBuildNumber()
        {
            IGenerator generator = CreateGenerator();
            generator.Execute();

            //var contents = File.ReadAllText(generator.ManifestPath);
            //Assert.Contains($"android:versionName=\"1.0.{generator.BuildNumber}\"", contents);
        }

        
    }
}
#pragma warning restore IDE0040 // Add accessibility modifiers
#pragma warning restore IDE1006 // Naming Styles
