using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Utils;
using Moq;
using Xunit;

namespace Mobile.BuildTools.Tests.Fixtures.Utils
{
    public class ImageSearchUtilityFixture : IDisposable
    {
        // private static readonly string ExpectedPath = Path.Combine(Environment.CurrentDirectory, "images", "test");
        private static readonly string ConfigPath = Path.Combine(Path.GetTempPath(), nameof(ImageSearchUtilityFixture));

        public ImageSearchUtilityFixture()
        {
            Directory.CreateDirectory(Path.Combine(ConfigPath, "images", "Debug"));
            Directory.CreateDirectory(Path.Combine(ConfigPath, "images", "release"));
            Directory.CreateDirectory(Path.Combine(ConfigPath, "images", "test"));
        }

        [Fact]
        public void PropertyHandlesSingleDirectoryPath()
        {
            var config = new BuildToolsConfig
            {
                Images = new ImageResize
                {
                    ConditionalDirectories = new Dictionary<string, IEnumerable<string>>(),
                    Directories = new List<string>
                    {
                        "images"
                    }
                }
            };

            var paths = ImageSearchUtil.GetSearchPaths(config, Platform.Android, "Debug", ConfigPath);

            Assert.Single(paths);
            Assert.Contains(paths, p => p == Path.Combine(ConfigPath, "images"));
        }

        [Fact]
        public void PropertyHandlesDirectoryPathWithForwardSlash()
        {
            var config = new BuildToolsConfig
            {
                Images = new ImageResize
                {
                    ConditionalDirectories = new Dictionary<string, IEnumerable<string>>(),
                    Directories = new List<string>
                    {
                        "images/test"
                    }
                }
            };

            var paths = ImageSearchUtil.GetSearchPaths(config, Platform.Android, "Debug", ConfigPath);

            Assert.Single(paths);
            Assert.Contains(paths, p => p == Path.Combine(ConfigPath, "images", "test"));
        }

        [Fact]
        public void PropertyHandlesDirectoryPathWithBackSlash()
        {
            var config = new BuildToolsConfig
            {
                Images = new ImageResize
                {
                    ConditionalDirectories = new Dictionary<string, IEnumerable<string>>(),
                    Directories = new List<string>
                    {
                        @"images\test"
                    }
                }
            };

            var paths = ImageSearchUtil.GetSearchPaths(config, Platform.Android, "Debug", ConfigPath);

            Assert.Single(paths);
            Assert.Contains(paths, p => p == Path.Combine(ConfigPath, "images", "test"));
        }

        [Theory]
        [InlineData("Debug", "Debug", 2)]
        [InlineData("Debug", "Release", 1)]
        [InlineData("Release", "Debug", 1)]
        [InlineData("Release", "Release", 2)]
        public void IncludesCompileConfigurationDirectory(string directory, string configuration, int expectedPaths)
        {
            var config = new BuildToolsConfig
            {
                Images = new ImageResize
                {
                    ConditionalDirectories = new Dictionary<string, IEnumerable<string>>
                    {
                        { directory, new[] { Path.Combine(ConfigPath, "images", directory )} },
                        { "Fake", new[] { Path.Combine(Environment.CurrentDirectory, "images", "Fake" )} },
                    },
                    Directories = new List<string>
                    {
                        "images"
                    }
                }
            };

            var paths = ImageSearchUtil.GetSearchPaths(config, Platform.Android, configuration, ConfigPath);

            Assert.Equal(expectedPaths, paths.Count());
            Assert.Contains(paths, p => p == Path.Combine(ConfigPath, "images"));
            if (expectedPaths > 1)
                Assert.Contains(paths, p => p == Path.Combine(ConfigPath, "images", directory));
        }

        [Fact]
        public void LogsAndRemovesPathsNotPresent()
        {
            var fakePath = Path.Combine(ConfigPath, "images", "Fake");
            var logger = new Mock<ILog>();
            var config = new BuildToolsConfig
            {
                Images = new ImageResize
                {
                    Directories = new List<string>
                    {
                        ConfigPath,
                        fakePath
                    }
                }
            };

            var paths = ImageSearchUtil.GetSearchPaths(config, Platform.Android, "Release", ConfigPath, logger: logger.Object);

            Assert.Single(paths);
            logger.Verify(x => x.LogWarning(It.IsAny<string>()));
        }

        void IDisposable.Dispose()
        {
            if (Directory.Exists(ConfigPath))
                Directory.Delete(ConfigPath, recursive: true);
        }
    }
}
