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
            var buildConfigurationSecrets = new
            {
                SampleProp = $"Hello {config.BuildConfiguration}"
            };
            File.WriteAllText(Path.Combine(config.ProjectDirectory, $"appsettings.{config.BuildConfiguration}.json"), JsonSerializer.Serialize(buildConfigurationSecrets));

            var mergedSecrets = EnvironmentAnalyzer.GatherEnvironmentVariables(config);

            Assert.True(mergedSecrets.ContainsKey("SampleProp"));
            Assert.Equal(buildConfigurationSecrets.SampleProp, mergedSecrets["SampleProp"]);
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
    }
}
