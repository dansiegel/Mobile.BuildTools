using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Tasks;
using Mobile.BuildTools.Tests.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures.Generators
{
    public class ScssGeneratorFixture : FixtureBase
    {
        private const string Templates = "Templates";
        private static readonly string ExpectedCssPath;
        private static readonly string Sass;
        private static readonly string Scss;

        static ScssGeneratorFixture()
        {
            ExpectedCssPath = $"{Templates}{Path.DirectorySeparatorChar}Css";
            Sass = $"{Templates}{Path.DirectorySeparatorChar}Sass";
            Scss = $"{Templates}{Path.DirectorySeparatorChar}Scss";
        }

        private IEnumerable<string> FilesToProcess { get; }

        public ScssGeneratorFixture(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
            FilesToProcess = Directory.GetFiles(Scss, "*");
        }

        [Fact]
        public void Generates_Two_CssFiles_FromScss()
        {
            var config = GetConfiguration();
            IGenerator<IEnumerable<string>> generator = new ScssGenerator(config)
            {
                InputFiles = FilesToProcess
            };

            var exception = Record.Exception(() =>
            {
                generator.Execute();
            });

            Assert.Null(exception);

            var outputs = generator.Outputs.ToArray();
            _testOutputHelper.WriteLine($"Generated Files: {outputs.Length}");
            Assert.Equal(2, outputs.Length);
            var files = Directory.GetFiles(config.IntermediateOutputPath, "*", SearchOption.AllDirectories);

            _testOutputHelper.WriteLine($"Files found: {string.Join(", ", files)}");
            Assert.Equal(2, files.Length);

            foreach (var item in generator.Outputs)
            {
                Assert.Contains(item, files);
            }
        }

        //[Fact]
        //public void Generates_One_CssFile_FromSass()
        //{
        //    ResetOutputFolder();

        //    var task = new ScssProcessorTask
        //    {
        //        OutputDirectory = OutputFolder,
        //        ScssFiles = Directory.GetFiles(Sass, "*"),
        //        Logger = new XunitLog(_testOutputHelper)
        //    };

        //    bool success = false;
        //    var exception = Record.Exception(() =>
        //    {
        //        success = task.Execute();
        //    });

        //    Assert.Null(exception);
        //    Assert.True(success);

        //    Assert.Single(task.GeneratedCssFiles);
        //    var files = Directory.GetFiles(OutputFolder, "*", SearchOption.AllDirectories);
        //    Assert.Single(files);
        //    Assert.Equal("Generated/Templates/Sass/style.css", files[0]);
        //}

        //[Fact]
        //public void Generates_Three_CssFiles_FromScssAndSass()
        //{
        //    ResetOutputFolder();

        //    var task = new ScssProcessorTask
        //    {
        //        OutputDirectory = OutputFolder,
        //        ScssFiles = Directory.GetFiles(Templates, "*", SearchOption.AllDirectories),
        //        Logger = new XunitLog(_testOutputHelper)
        //    };

        //    bool success = false;
        //    var exception = Record.Exception(() =>
        //    {
        //        success = task.Execute();
        //    });

        //    Assert.Null(exception);
        //    Assert.True(success);

        //    _testOutputHelper.WriteLine($"There should be {task.GeneratedCssFiles.Length} generated files");
        //    Assert.Equal(3, task.GeneratedCssFiles.Length);
        //    var files = Directory.GetFiles(OutputFolder, "*", SearchOption.AllDirectories);
        //    Assert.Equal(3, files.Length);
        //    Assert.Contains("Generated/Templates/Sass/style.css", files);
        //    Assert.Contains("Generated/Templates/Scss/style.css", files);
        //    Assert.Contains("Generated/Templates/Scss/style2.css", files);
        //}

        //[Fact]
        //public void GeneratedExpectedCss_FromSass()
        //{
        //    var task = new ScssProcessorTask
        //    {
        //        OutputDirectory = OutputFolder,
        //        ScssFiles = Directory.GetFiles(Sass, "*"),
        //        Logger = new XunitLog(_testOutputHelper)
        //    };

        //    bool success = false;
        //    var exception = Record.Exception(() =>
        //    {
        //        success = task.Execute();
        //    });

        //    Assert.Null(exception);
        //    Assert.True(success);

        //    Assert.Single(task.GeneratedCssFiles);

        //    var files = Directory.GetFiles(OutputFolder, "style.css", SearchOption.AllDirectories);
        //    Assert.Single(files);
        //    var generatedFile = files.First();
        //    Assert.Equal(File.ReadAllText(ExpectedCssPath), File.ReadAllText(generatedFile));
        //}

        [Theory]
        [InlineData("style.css", "style.min.css")]
        [InlineData("style2.css", "style2.min.css")]
        public void GeneratedExpectedCss_FromScss(string fileName, string expectedFileName)
        {
            _testOutputHelper.WriteLine($"Checking: {fileName} - {expectedFileName}");
            var config = GetConfiguration();
            IGenerator<IEnumerable<string>> generator = new ScssGenerator(config)
            {
                InputFiles = FilesToProcess
            };

            var exception = Record.Exception(() =>
            {
                generator.Execute();
            });

            Assert.Null(exception);

            Assert.Equal(2, generator.Outputs.Count());

            var files = Directory.GetFiles(config.IntermediateOutputPath, fileName, SearchOption.AllDirectories);
            Assert.Single(files);
            var generatedFile = files.First(f => f.EndsWith(fileName, StringComparison.CurrentCultureIgnoreCase));
            Assert.Equal(File.ReadLines(Path.Combine(ExpectedCssPath, expectedFileName)).First(),
                         File.ReadLines(generatedFile).First());
        }

        [Fact]
        public void NoOutputDoesNotCauseNullOutputCollection()
        {
            var config = GetConfiguration();
            IGenerator<IEnumerable<string>> generator = new ScssGenerator(config)
            {
                InputFiles = Directory.GetFiles(ExpectedCssPath, "*"),
            };

            var exception = Record.Exception(() =>
            {
                generator.Execute();
            });

            Assert.Null(exception);
            Assert.NotNull(generator.Outputs);
            Assert.Empty(generator.Outputs);
        }
    }
}