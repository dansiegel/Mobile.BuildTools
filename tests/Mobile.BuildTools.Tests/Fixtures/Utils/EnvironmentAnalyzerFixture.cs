﻿using System.Text.Json;
using Mobile.BuildTools.Models.Settings;
using Mobile.BuildTools.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures.Utils
{
    public class EnvironmentAnalyzerFixture(ITestOutputHelper testOutputHelper) : FixtureBase(null, testOutputHelper), IDisposable
    {
        [Theory]
        [InlineData("secrets.json")]
        [InlineData("appsettings.json")]
        public void GetsValuesFromJson(string filename)
        {
            var config = GetConfiguration($"{nameof(GetsValuesFromJson)}-{Path.GetFileNameWithoutExtension(filename)}");
            var settingsConfig = new SettingsConfig
            {
                Properties =
                    [
                        new ValueConfig
                        {
                            Name = "SampleProp",
                            PropertyType = PropertyType.String
                        }
                    ]
            };
            config.Configuration.AppSettings[config.ProjectName] = new List<SettingsConfig>([settingsConfig]);

            var secrets = new
            {
                SampleProp = "Hello Tests"
            };
            File.WriteAllText(Path.Combine(config.ProjectDirectory, filename), JsonSerializer.Serialize(secrets));

            var mergedSecrets = EnvironmentAnalyzer.GatherEnvironmentVariables(config);

            Assert.True(mergedSecrets.ContainsKey("SampleProp"));
            Assert.Equal(secrets.SampleProp, mergedSecrets["SampleProp"]);
        }

        [Fact]
        public void GetsValueFromBuildConfigurationOverrideJson()
        {
            var config = GetConfiguration();
            config.BuildConfiguration = "Debug";
            var settingsConfig = new SettingsConfig
            {
                Properties =
                    [
                        new ValueConfig
                        {
                            Name = "SampleProp",
                            PropertyType = PropertyType.String
                        }
                    ]
            };

            config.Configuration.AppSettings[config.ProjectName] = new List<SettingsConfig>([settingsConfig]);

            var secrets = new
            {
                SampleProp = "Hello Tests"
            };
            File.WriteAllText(Path.Combine(config.ProjectDirectory, "appsettings.json"), JsonSerializer.Serialize(secrets));
            var debugBuildSecrets = new
            {
                SampleProp = $"Hello Debug"
            };
            File.WriteAllText(Path.Combine(config.ProjectDirectory, $"appsettings.Debug.json"), JsonSerializer.Serialize(debugBuildSecrets)); 
            var releaseBuildSecrets = new
            {
                SampleProp = $"Hello Release"
            };
            File.WriteAllText(Path.Combine(config.ProjectDirectory, $"appsettings.Release.json"), JsonSerializer.Serialize(releaseBuildSecrets));

            var mergedSecrets = EnvironmentAnalyzer.GatherEnvironmentVariables(config);

            Assert.True(mergedSecrets.ContainsKey("SampleProp"));
            Assert.Equal(debugBuildSecrets.SampleProp, mergedSecrets["SampleProp"]);
        }

        [Theory]
        [InlineData("DebugQA", "Debug")]
        [InlineData("DebugQA", "QA")]
        public void GetsValueFromBuildFuzzyConfigurationOverrideJson(string buildConfiguration, string fuzzyConfiguration)
        {
            var config = GetConfiguration();
            config.BuildConfiguration = buildConfiguration;
            config.Configuration.Environment ??= new BuildTools.Models.EnvironmentSettings();
            config.Configuration.Environment.EnableFuzzyMatching = true;

            var secrets = new
            {
                SampleProp = "Hello Tests"
            };
            File.WriteAllText(Path.Combine(config.ProjectDirectory, "appsettings.json"), JsonSerializer.Serialize(secrets));
            var debugBuildSecrets = new
            {
                SampleProp = $"Hello {fuzzyConfiguration}"
            };
            File.WriteAllText(Path.Combine(config.ProjectDirectory, $"appsettings.{fuzzyConfiguration}.json"), JsonSerializer.Serialize(debugBuildSecrets));
            var releaseBuildSecrets = new
            {
                SampleProp = "Hello Release"
            };
            File.WriteAllText(Path.Combine(config.ProjectDirectory, $"appsettings.Release.json"), JsonSerializer.Serialize(releaseBuildSecrets));

            var mergedSecrets = EnvironmentAnalyzer.GatherEnvironmentVariables(config);

            Assert.True(mergedSecrets.ContainsKey("SampleProp"));
            Assert.Equal(debugBuildSecrets.SampleProp, mergedSecrets["SampleProp"]);
        }

        [Fact]
        public void GetsValuesFromConfigurationEnvironment()
        {
            var config = GetConfiguration();
            var settingsConfig = new SettingsConfig
            {
                Properties =
                [
                    new ValueConfig
                    {
                        Name = "SampleProp",
                        PropertyType = PropertyType.String
                    }
                ]
            };
            config.Configuration.Environment.Defaults.Add("SampleProp", "Hello Tests");
            config.Configuration.AppSettings[config.ProjectName] = new List<SettingsConfig>([settingsConfig]);

            var mergedSecrets = EnvironmentAnalyzer.GatherEnvironmentVariables(config);

            Assert.True(mergedSecrets.ContainsKey("SampleProp"));
            Assert.Equal(config.Configuration.Environment.Defaults["SampleProp"], mergedSecrets["SampleProp"]);
        }

        [Fact]
        public void GetsValueFromProcessEnvironment()
        {
            var expectedValue = Guid.NewGuid().ToString();
            var key = nameof(GetsValueFromProcessEnvironment);
            Environment.SetEnvironmentVariable(key, expectedValue, EnvironmentVariableTarget.Process);

            var config = GetConfiguration();

            var mergedSecrets = EnvironmentAnalyzer.GatherEnvironmentVariables(config);

            Assert.True(mergedSecrets.TryGetValue(key, out var actualValue));
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void GetsValuesForBuildConfigurationFromConfigurationEnvironment()
        {
            var config = GetConfiguration();
            var settingsConfig = new SettingsConfig
            {
                Properties =
                [
                    new ValueConfig
                    {
                        Name = "SampleProp",
                        PropertyType = PropertyType.String
                    }
                ]
            };
            config.Configuration.Environment.Defaults.Add("SampleProp", "Hello Tests");
            config.Configuration.Environment.Configuration[config.BuildConfiguration] = new Dictionary<string, string>
            {
                { "SampleProp", "Hello Override" }
            };
            config.Configuration.AppSettings[config.ProjectName] = new List<SettingsConfig>([settingsConfig]);

            var mergedSecrets = EnvironmentAnalyzer.GatherEnvironmentVariables(config);

            Assert.True(mergedSecrets.ContainsKey("SampleProp"));
            Assert.Equal(config.Configuration.Environment.Configuration[config.BuildConfiguration]["SampleProp"], mergedSecrets["SampleProp"]);
        }

        [Fact]
        public void GetsValuesFromHostEnvironment()
        {
            var config = GetConfiguration();
            var settingsConfig = new SettingsConfig
            {
                Properties =
                [
                    new ValueConfig
                    {
                        Name = "SampleProp1",
                        PropertyType = PropertyType.String
                    }
                ]
            };
            config.Configuration.AppSettings[config.ProjectName] = new List<SettingsConfig>([settingsConfig]);
            Environment.SetEnvironmentVariable("SampleProp1", nameof(GetsValuesFromHostEnvironment), EnvironmentVariableTarget.Process);

            var mergedSecrets = EnvironmentAnalyzer.GatherEnvironmentVariables(config);

            Assert.True(mergedSecrets.ContainsKey("SampleProp1"));
            Assert.Equal(nameof(GetsValuesFromHostEnvironment), mergedSecrets["SampleProp1"]);
        }

        [Fact]
        public void OverridesConfigEnvironmentFromHostEnvironment()
        {
            var config = GetConfiguration();
            var settingsConfig = new SettingsConfig
            {
                Properties =
                [
                    new ValueConfig
                    {
                        Name = "SampleProp3",
                        PropertyType = PropertyType.String
                    }
                ]
            };
            config.Configuration.AppSettings[config.ProjectName] = new List<SettingsConfig>([settingsConfig]);
            config.Configuration.Environment.Defaults["SampleProp3"] = "Hello Config Environment";

            Environment.SetEnvironmentVariable("SampleProp3", nameof(OverridesConfigEnvironmentFromHostEnvironment), EnvironmentVariableTarget.Process);

            var mergedSecrets = EnvironmentAnalyzer.GatherEnvironmentVariables(config);

            Assert.True(mergedSecrets.ContainsKey("SampleProp3"));
            Assert.Equal(nameof(OverridesConfigEnvironmentFromHostEnvironment), mergedSecrets["SampleProp3"]);
        }

        [Theory]
        [InlineData("secrets.json")]
        [InlineData("appsettings.json")]
        public void OverridesConfigEnvironmentFromJson(string filename)
        {
            var config = GetConfiguration($"{nameof(OverridesConfigEnvironmentFromJson)}-{Path.GetFileNameWithoutExtension(filename)}");
            var settingsConfig = new SettingsConfig
            {
                Properties =
                [
                    new ValueConfig
                    {
                        Name = "SampleProp",
                        PropertyType = PropertyType.String
                    }
                ]
            };
            config.Configuration.AppSettings[config.ProjectName] = new List<SettingsConfig>([settingsConfig]);
            config.Configuration.Environment.Defaults["SampleProp"] = "Hello Config Environment";
            var secrets = new
            {
                SampleProp = "Hello Tests"
            };
            File.WriteAllText(Path.Combine(config.ProjectDirectory, filename), JsonSerializer.Serialize(secrets));

            var mergedSecrets = EnvironmentAnalyzer.GatherEnvironmentVariables(config);

            Assert.True(mergedSecrets.ContainsKey("SampleProp"));
            Assert.Equal(secrets.SampleProp, mergedSecrets["SampleProp"]);
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable("SampleProp", null, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("SampleProp1", null, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("SampleProp2", null, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("SampleProp3", null, EnvironmentVariableTarget.Process);
        }

        [Theory]
        [InlineData("Debug", Platform.Unsupported, "Debug", false)]
        [InlineData("DebugApk", Platform.Unsupported, "Debug", true)]
        [InlineData("QA", Platform.Unsupported, "QA", false)]
        [InlineData("Release", Platform.Unsupported, "Release", false)]
        [InlineData("Debug", Platform.Android, "Android_Debug", false)]
        [InlineData("Debug", Platform.iOS, "iOS_Debug", false)]
        [InlineData("Stage", Platform.Android, "Android", false)]
        public void GetsValuesFromBuildConfiguration(string buildConfiguration, Platform platform, string expectedEnvironment, bool fuzzyMatching)
        {
            var config = GetConfiguration($"{nameof(GetsValuesFromBuildConfiguration)}-{buildConfiguration}");
            config.BuildConfiguration = buildConfiguration;
            config.Platform = platform;
            config.Configuration.Environment.EnableFuzzyMatching = fuzzyMatching;
            const string key = "EnvironmentProp";

            config.Configuration.Environment.Configuration["Debug"] = new Dictionary<string, string>
            {
                { key, "Hello Debug" }
            };
            config.Configuration.Environment.Configuration["Android_Debug"] = new Dictionary<string, string>
            {
                { key, "Hello Android Debug" }
            };
            config.Configuration.Environment.Configuration["iOS_Debug"] = new Dictionary<string, string>
            {
                { key, "Hello iOS Debug" }
            };
            config.Configuration.Environment.Configuration["QA"] = new Dictionary<string, string>
            {
                { key, "Hello QA" }
            };
            config.Configuration.Environment.Configuration["Release"] = new Dictionary<string, string>
            {
                { key, "Hello Release" }
            };
            config.Configuration.Environment.Configuration["Android"] = new Dictionary<string, string>
            {
                { key, "Hello Android" }
            };
            config.Configuration.Environment.Configuration["Stage"] = new Dictionary<string, string>
            {
                { key, "Hello Stage" }
            };
            var expectedValue = config.Configuration.Environment.Configuration[expectedEnvironment][key];

            var mergedSecrets = EnvironmentAnalyzer.GatherEnvironmentVariables(config);
            Assert.True(mergedSecrets.ContainsKey(key));
            Assert.Equal(expectedValue, mergedSecrets[key]);
        }
    }
}
