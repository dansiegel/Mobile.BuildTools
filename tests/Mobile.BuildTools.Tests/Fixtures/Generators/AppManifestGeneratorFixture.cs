using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Generators.Manifests;
using Mobile.BuildTools.Tests.Mocks;
using Mobile.BuildTools.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures.Generators
{
    public class AppManifestGeneratorFixture : FixtureBase
    {
        private const string TestPrefix = "XunitTest_";
        private static readonly string TemplateAndroidManifestPath = @"Templates/MockAndroidManifest.xml";
        private static readonly string TemplateAndroidManifestOutputPath = @"Generated/AndroidManifest.xml";
        private static readonly string TemplateManifestPath = @"Templates/MockManifestTemplate.json";
        private static readonly string TemplateManifestOutputPath = @"Generated/MockManifest.json";

        public AppManifestGeneratorFixture(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
            Environment.SetEnvironmentVariable($"{TestPrefix}TemplatedParameter", nameof(AppManifestGeneratorFixture));
            Environment.SetEnvironmentVariable($"{TestPrefix}CustomTokenParameter", nameof(AppManifestGeneratorFixture));
        }

        private BaseTemplatedManifestGenerator CreateGenerator() =>
            CreateGenerator(GetConfiguration());

        private BaseTemplatedManifestGenerator CreateGenerator(TestBuildConfiguration config)
        {
            config.Configuration.Manifests.VariablePrefix = TestPrefix;
            return new DefaultTemplatedManifestGenerator(config)
            {
                ManifestOutputPath = TemplateManifestOutputPath,
            };
        }

        [Fact]
        public void GetMatches()
        {
            var generator = CreateGenerator();
            var template = File.ReadAllText(TemplateManifestPath);
            _testOutputHelper.WriteLine(template);
            var matches = generator.GetMatches(template);
            Assert.NotNull(matches);
            Assert.NotEmpty(matches);
            Assert.Single(matches);
            var match = matches.Cast<Match>().FirstOrDefault();
            Assert.Equal("$TemplatedParameter$", match.Value);
        }

        [Fact]
        public void ProcessesStandardTokenizedVariables()
        {
            var generator = CreateGenerator();
            var template = File.ReadAllText(TemplateManifestPath);
            var match = generator.GetMatches(template).Cast<Match>().FirstOrDefault();
            var config = new TestBuildConfiguration
            {
                BuildConfiguration = "Test",
                ProjectDirectory = Directory.GetCurrentDirectory(),
                SolutionDirectory = Directory.GetCurrentDirectory()
            };
            var variables = EnvironmentAnalyzer.GatherEnvironmentVariables(config, true);
            foreach(var variable in variables)
            {
                _testOutputHelper.WriteLine($"  - {variable.Key}: {variable.Value}");
            }
            var processedTemplate = generator.ProcessMatch(template, match, variables);
            var json = JsonSerializer.Deserialize<TestManifest>(processedTemplate);

            Assert.Equal("%%CustomTokenParameter%%", json.CustomTokenParameter);
            Assert.Equal(nameof(AppManifestGeneratorFixture), json.TemplatedParameter);
        }

        [Fact]
        public void ProcessCustomTokenizedVariables()
        {
            var config = GetConfiguration();
            config.BuildConfiguration = "Test";
            config.ProjectDirectory = config.SolutionDirectory = Directory.GetCurrentDirectory();
            config.Configuration.Manifests.Token = @"%%";
            var generator = CreateGenerator(config);

            var template = File.ReadAllText(TemplateManifestPath);
            var match = generator.GetMatches(template).Cast<Match>().FirstOrDefault();
            var variables = EnvironmentAnalyzer.GatherEnvironmentVariables(config, true);
            var processedTemplate = generator.ProcessMatch(template, match, variables);
            var json = JsonSerializer.Deserialize<TestManifest>(processedTemplate);

            Assert.Equal(nameof(AppManifestGeneratorFixture), json.CustomTokenParameter);
            Assert.Equal("$TemplatedParameter$", json.TemplatedParameter);
        }

        [Fact(Skip = "Dunno")]
        public void ProcessingDoesNotCorruptAndroidManifest()
        {
            var config = GetConfiguration();
            var generator = CreateGenerator(config);
            var template = File.ReadAllText(TemplateAndroidManifestPath);
            var guid = Guid.NewGuid().ToString();
            Environment.SetEnvironmentVariable("XunitTest_AADClientId", guid);

            var matches = generator.GetMatches(template);
            var match = matches.Cast<Match>().First();
            Assert.Equal("$$AADClientId$$", match.Value);

            generator.ManifestOutputPath = TemplateAndroidManifestOutputPath;
            config.Configuration.Debug = true;

            var ex = Record.Exception(() => ((IGenerator)generator).Execute());
            Assert.Null(ex);

            var generatedTemplate = File.ReadAllText(TemplateAndroidManifestOutputPath);
            ex = Record.Exception(() =>
            {
                var doc = new XmlDocument();
                doc.LoadXml(generatedTemplate);
            });

            Assert.Null(ex);

            Assert.DoesNotContain("msal$$AADClientId$$", generatedTemplate);
            Assert.Contains($"msal{guid}", generatedTemplate);
        }

        private class TestManifest
        {
            public string TemplatedParameter { get; set; }

            public string CustomTokenParameter { get; set; }
        }
    }
}
