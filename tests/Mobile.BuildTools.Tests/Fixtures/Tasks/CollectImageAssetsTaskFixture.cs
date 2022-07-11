﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Tasks;
using Mobile.BuildTools.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures.Tasks
{
    public class CollectImageAssetsTaskFixture : FixtureBase, IDisposable
    {
        private static readonly string ConfigPath = Path.Combine(Path.GetTempPath(), nameof(CollectImageAssetsTaskFixture));

        public CollectImageAssetsTaskFixture(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
            Directory.CreateDirectory(Path.Combine(ConfigPath, "images", "debug"));
            Directory.CreateDirectory(Path.Combine(ConfigPath, "images", "release"));
            Directory.CreateDirectory(Path.Combine(ConfigPath, "images", "sample"));
            Directory.CreateDirectory(Path.Combine(ConfigPath, "PlatformSpecific"));
            Directory.CreateDirectory(Path.Combine(ConfigPath, "ShouldNotBeIncluded"));
            Directory.CreateDirectory(Path.Combine(ConfigPath, "DebugSpecific"));
            Directory.CreateDirectory(Path.Combine(ConfigPath, "ReleaseSpecific"));
            Directory.CreateDirectory(Path.Combine(ConfigPath, "StoreSpecific"));
            Directory.CreateDirectory(Path.Combine(ConfigPath, "NotDebug"));
            Directory.CreateDirectory(Path.Combine(ConfigPath, "Sample"));
        }

        [Fact]
        public void NullImageConfigurationReturnsEmptyPaths()
        {
            var config = GetConfiguration();
            config.Platform = Platform.iOS;
            config.Configuration.Images = null;
            var task = new CollectImageAssetsTask();
            IEnumerable<string> paths = null;
            var ex = Record.Exception(() => paths = task.GetSearchPaths(config));

            Assert.Null(ex);
            Assert.Empty(paths);
        }

        [Fact]
        public void CollectsSingleSearchDirectory()
        {
            var config = GetConfiguration();
            config.Platform = Platform.iOS;
            config.Configuration.Images = new ImageResize
            {
                Directories = new List<string> { "Images" },
                ConditionalDirectories = null
            };

            var task = new CollectImageAssetsTask
            {
                ConfigurationPath = ConfigPath
            };

            IEnumerable<string> paths = null;
            var ex = Record.Exception(() => paths = task.GetSearchPaths(config));

            Assert.Null(ex);
            Assert.Single(paths);
            Assert.Equal(Path.Combine(task.ConfigurationPath, "Images"), paths.First());
        }

        [Fact]
        public void CollectsMultipleSearchDirectories()
        {
            var config = GetConfiguration();
            config.Platform = Platform.iOS;
            config.Configuration.Images = new ImageResize
            {
                Directories = new List<string> { "Images", "Sample" },
                ConditionalDirectories = null
            };

            var task = new CollectImageAssetsTask
            {
                ConfigurationPath = ConfigPath
            };

            IEnumerable<string> paths = null;
            var ex = Record.Exception(() => paths = task.GetSearchPaths(config));

            Assert.Null(ex);
            Assert.Equal(2, paths.Count());
            var expectedPaths = new[] { Path.Combine(task.ConfigurationPath, "Images"), Path.Combine(task.ConfigurationPath, "Sample") };
            foreach (var expected in expectedPaths)
                Assert.Contains(expected, paths);
        }

        [Theory]
        [InlineData(Platform.iOS, "ios")]
        [InlineData(Platform.iOS, "apple")]
        [InlineData(Platform.iOS, "xamarin.ios")]
        [InlineData(Platform.iOS, "xamarinios")]
        [InlineData(Platform.Android, "droid")]
        [InlineData(Platform.Android, "android")]
        [InlineData(Platform.Android, "monoandroid")]
        public void PicksupPlatformSpecificDirectory(Platform platform, string key)
        {
            var config = GetConfiguration();
            config.Platform = platform;
            config.Configuration.Images = new ImageResize
            {
                Directories = new List<string> { "Images" },
                ConditionalDirectories = new Dictionary<string, IEnumerable<string>>
                {
                    { key, new[] { "PlatformSpecific" } },
                    { "Foo", new[] { "ShouldNotBeIncluded" } }
                }
            };

            var task = new CollectImageAssetsTask
            {
                ConfigurationPath = ConfigPath
            };

            IEnumerable<string> paths = null;
            var ex = Record.Exception(() => paths = task.GetSearchPaths(config));

            Assert.Null(ex);
            Assert.Equal(2, paths.Count());
            Assert.DoesNotContain(Path.Combine(task.ConfigurationPath, "ShouldNotBeIncluded"), paths);
            var expectedPaths = new[] { Path.Combine(task.ConfigurationPath, "Images"), Path.Combine(task.ConfigurationPath, "PlatformSpecific") };
            foreach (var expected in expectedPaths)
                Assert.Contains(expected, paths);
        }

        [Fact]
        public void CollectsBuildConfigurationConditionalDirectory()
        {
            var config = GetConfiguration();
            config.BuildConfiguration = "Debug";
            config.Configuration.Images = new ImageResize
            {
                Directories = new List<string> { "Images" },
                ConditionalDirectories = new Dictionary<string, IEnumerable<string>>
                {
                    { "Debug", new[] { "DebugSpecific" } },
                    { "Release", new[] { "ReleaseSpecific" } },
                    { "Store", new[] { "StoreSpecific" } }
                }
            };

            var task = new CollectImageAssetsTask
            {
                ConfigurationPath = ConfigPath
            };

            IEnumerable<string> paths = null;
            var ex = Record.Exception(() => paths = task.GetSearchPaths(config));

            Assert.Null(ex);
            Assert.Equal(2, paths.Count());

            var expectedPaths = new[] { Path.Combine(task.ConfigurationPath, "Images"), Path.Combine(task.ConfigurationPath, "DebugSpecific") };
            foreach (var expected in expectedPaths)
                Assert.Contains(expected, paths);
        }

        [Fact]
        public void CollectsNegatedBuildConfigurationConditionalDirectory()
        {
            var config = GetConfiguration();
            config.BuildConfiguration = "Release";
            config.Configuration.Images = new ImageResize
            {
                Directories = new List<string> { "Images" },
                ConditionalDirectories = new Dictionary<string, IEnumerable<string>>
                {
                    { "!Debug", new[] { "NotDebug" } },
                    { "Store", new[] { "StoreSpecific" } }
                }
            };

            var task = new CollectImageAssetsTask
            {
                ConfigurationPath = ConfigPath
            };

            IEnumerable<string> paths = null;
            var ex = Record.Exception(() => paths = task.GetSearchPaths(config));

            Assert.Null(ex);
            Assert.Equal(2, paths.Count());

            var expectedPaths = new[] { Path.Combine(task.ConfigurationPath, "Images"), Path.Combine(task.ConfigurationPath, "NotDebug") };
            foreach (var expected in expectedPaths)
                Assert.Contains(expected, paths);
        }

        [Fact]
        public void ReturnsDistinctPaths()
        {
            var config = GetConfiguration();
            config.BuildConfiguration = "Debug";
            config.Configuration.Images = new ImageResize
            {
                Directories = new List<string> { "Images" },
                ConditionalDirectories = new Dictionary<string, IEnumerable<string>>
                {
                    { "Debug", new[] { "Images" } },
                }
            };

            var task = new CollectImageAssetsTask
            {
                ConfigurationPath = ConfigPath
            };

            IEnumerable<string> paths = null;
            var ex = Record.Exception(() => paths = task.GetSearchPaths(config));

            Assert.Null(ex);
            Assert.Single(paths);
            Assert.Equal(Path.Combine(task.ConfigurationPath, "Images"), paths.First());
        }

        void IDisposable.Dispose()
        {
            if (Directory.Exists(ConfigPath))
                Directory.Delete(ConfigPath, recursive: true);
        }
    }
}
