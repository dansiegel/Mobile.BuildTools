using System.Diagnostics;
using System.IO;
using Mobile.BuildTools.Tests.Mocks;
using Mobile.BuildTools.Utils;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures
{
    public abstract class FixtureBase
    {
        protected ITestOutputHelper _testOutputHelper { get; }
        protected string ProjectDirectory { get; }

        protected FixtureBase(ITestOutputHelper testOutputHelper)
            : this(null, testOutputHelper)
        {
        }

        protected FixtureBase(string projectDirectory, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            ProjectDirectory = projectDirectory;
        }

        protected TestBuildConfiguration GetConfiguration(string testName = null)
        {
            if(string.IsNullOrEmpty(testName))
            {
                var stackTrace = new StackTrace();
                testName = stackTrace.GetFrame(1).GetMethod().Name;
            }

            if (testName.Length > 20)
                testName = System.Guid.NewGuid().ToString();

            var tempDir = Path.GetTempPath();
            var projectDirectory = ProjectDirectory;
            var solutionDirectory = ProjectDirectory;
            if (string.IsNullOrEmpty(projectDirectory))
            {
                projectDirectory = Path.Combine(tempDir, "Tests", GetType().Name, testName, "SolutionDir", "src", testName);
                solutionDirectory = Path.Combine(tempDir, "Tests", GetType().Name, testName, "SolutionDir");
            }

            var testOutput = Path.Combine(tempDir, "Tests", GetType().Name, testName);
            ResetTestOutputDirectory(testOutput);
            ResetTestOutputDirectory(projectDirectory);

            return new TestBuildConfiguration
            {
                Logger = new XunitLog(_testOutputHelper),
                Platform = Platform.Unsupported,
                IntermediateOutputPath = testOutput,
                ProjectDirectory = projectDirectory,
                SolutionDirectory = solutionDirectory,
                BuildConfiguration = "Debug",
                ProjectName = "AwesomeApp",
            };
        }

        private void ResetTestOutputDirectory(string path)
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch { }
            finally
            {
                Directory.CreateDirectory(path);
            }
        }

        protected static void DirectoryCopy(string sourceDirName, string destDirName)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            var dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                if (file.Name == ".DS_Store")
                    continue;

                var temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // copy subdirectories, copy them and their contents to new location.
            foreach (var subdir in dirs)
            {
                var temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }
        }
    }
}
