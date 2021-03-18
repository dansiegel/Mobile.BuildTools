using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Utils;
using Xunit;

namespace Mobile.BuildTools.Tests.Fixtures.Utils
{
    public class ImageSearchUtilityFixture
    {
        private static readonly string ExpectedPath = Path.Combine(Environment.CurrentDirectory, "images", "test");

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

            var paths = ImageSearchUtil.GetSearchPaths(config, Platform.Android, "Debug", Environment.CurrentDirectory);

            Assert.Single(paths);
            Assert.Contains(paths, p => p == Path.Combine(Environment.CurrentDirectory, "images"));
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

            var paths = ImageSearchUtil.GetSearchPaths(config, Platform.Android, "Debug", Environment.CurrentDirectory);

            Assert.Single(paths);
            Assert.Contains(paths, p => p == Path.Combine(Environment.CurrentDirectory, "images", "test"));
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

            var paths = ImageSearchUtil.GetSearchPaths(config, Platform.Android, "Debug", Environment.CurrentDirectory);

            Assert.Single(paths);
            Assert.Contains(paths, p => p == Path.Combine(Environment.CurrentDirectory, "images", "test"));
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
                        { directory, new[] { Path.Combine(Environment.CurrentDirectory, "images", directory )} },
                        { "Fake", new[] { Path.Combine(Environment.CurrentDirectory, "images", "Fake" )} },
                    },
                    Directories = new List<string>
                    {
                        "images"
                    }
                }
            };

            var paths = ImageSearchUtil.GetSearchPaths(config, Platform.Android, configuration, Environment.CurrentDirectory);

            Assert.Equal(expectedPaths, paths.Count());
            Assert.Contains(paths, p => p == Path.Combine(Environment.CurrentDirectory, "images"));
            if (expectedPaths > 1)
                Assert.Contains(paths, p => p == Path.Combine(Environment.CurrentDirectory, "images", directory));
        }
    }
}
