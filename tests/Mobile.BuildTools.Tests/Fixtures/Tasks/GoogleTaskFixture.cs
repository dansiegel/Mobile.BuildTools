using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mobile.BuildTools.Tasks;
using Xunit;

namespace Mobile.BuildTools.Tests.Fixtures.Tasks
{
    public class GoogleTaskFixture
    {
        private readonly string IntermediateOutputDirectory = new FileInfo(Path.Combine("Generated", "GoogleTask")).FullName;

        [Fact]
        public void ReturnsIntermediateFileFromVariable()
        {
            var contents = "SampleGoogleServices";
            var environmentName = nameof(ReturnsIntermediateFileFromVariable);
            Environment.SetEnvironmentVariable(environmentName, contents, EnvironmentVariableTarget.Process);

            var path = GoogleTask.GetResourcePath(environmentName, IntermediateOutputDirectory, "google-services.json");

            var expectedPath = Path.Combine(IntermediateOutputDirectory, "google-services", "google-services.json");

            Assert.Equal(expectedPath, path);

            Assert.Equal(contents, File.ReadAllText(path));
        }

        [Fact]
        public void ReturnsSpecifiedFileFromRelativePath()
        {
            var contents = "SampleGoogleServices";
            var directory = Path.Combine("Generated", "tests", nameof(ReturnsSpecifiedFileFromRelativePath));
            Directory.CreateDirectory(directory);
            var expectedPath = Path.Combine(directory, "google-test.json");
            File.WriteAllText(expectedPath, contents);
            var environmentName = nameof(ReturnsSpecifiedFileFromRelativePath);
            Environment.SetEnvironmentVariable(environmentName, expectedPath, EnvironmentVariableTarget.Process);

            var path = GoogleTask.GetResourcePath(environmentName, IntermediateOutputDirectory, "google-services.json");

            Assert.Equal(new FileInfo(expectedPath).FullName, path);
        }

        [Fact]
        public void ReturnsSpecifiedFileFromAbsolutePath()
        {
            var contents = "SampleGoogleServices";
            var directory = Path.Combine("Generated", "tests", nameof(ReturnsSpecifiedFileFromAbsolutePath));
            Directory.CreateDirectory(directory);
            var expectedPath = new FileInfo(Path.Combine(directory, "google-test.json")).FullName;
            File.WriteAllText(expectedPath, contents);
            var environmentName = nameof(ReturnsSpecifiedFileFromAbsolutePath);
            Environment.SetEnvironmentVariable(environmentName, expectedPath, EnvironmentVariableTarget.Process);

            var path = GoogleTask.GetResourcePath(environmentName, IntermediateOutputDirectory, "google-services.json");

            Assert.Equal(expectedPath, path);
        }
    }
}
