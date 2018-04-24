using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mobile.BuildTools.Tasks;
using Mobile.BuildTools.Tests.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures
{
    public class ScssProcessorTaskFixture
    {
        private const string OutputFolder = "Generated";
        private const string Templates = "Templates";
        private const string ExpectedCssPath = "Templates/Css";
        private const string Sass = "Templates/Sass";
        private const string Scss = "Templates/Scss";

        private ITestOutputHelper _testOutputHelper { get; }

        public ScssProcessorTaskFixture(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            ResetOutputFolder();
        }

        [Fact]
        public void Generates_Two_CssFiles_FromScss()
        {
            ResetOutputFolder();

            var task = new ScssProcessorTask
            {
                OutputDirectory = OutputFolder,
                NoneIncluded = Directory.GetFiles(Scss, "*"),
                Logger = new XunitLog(_testOutputHelper)
            };

            bool success = false;
            var exception = Record.Exception(() =>
            {
                success = task.Execute();
            });

            Assert.Null(exception);
            Assert.True(success);

            _testOutputHelper.WriteLine($"Generated Files: {task.GeneratedCssFiles.Length}");
            Assert.Equal(2, task.GeneratedCssFiles.Length);
            var files = Directory.GetFiles(OutputFolder, "*", SearchOption.AllDirectories);

            _testOutputHelper.WriteLine($"Files found: {string.Join(", ", files)}");
            Assert.Equal(2, files.Length);
            Assert.Contains("Generated/Templates/Scss/style.css", files);
            Assert.Contains("Generated/Templates/Scss/style2.css", files);
        }

        //[Fact]
        //public void Generates_One_CssFile_FromSass()
        //{
        //    ResetOutputFolder();

        //    var task = new ScssProcessorTask
        //    {
        //        OutputDirectory = OutputFolder,
        //        NoneIncluded = Directory.GetFiles(Sass, "*"),
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
        //        NoneIncluded = Directory.GetFiles(Templates, "*", SearchOption.AllDirectories),
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
        //        NoneIncluded = Directory.GetFiles(Sass, "*"),
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
        [InlineData("style.css")]
        [InlineData("style2.css")]
        public void GeneratedExpectedCss_FromScss(string fileName)
        {
            _testOutputHelper.WriteLine($"Checking: {fileName}");
            var task = new ScssProcessorTask
            {
                OutputDirectory = OutputFolder,
                NoneIncluded = Directory.GetFiles(Scss, "*"),
                Logger = new XunitLog(_testOutputHelper)
            };

            bool success = false;
            var exception = Record.Exception(() =>
            {
                success = task.Execute();
            });

            Assert.Null(exception);
            Assert.True(success);

            Assert.Equal(2, task.GeneratedCssFiles.Length);

            var files = Directory.GetFiles(OutputFolder, fileName, SearchOption.AllDirectories);
            Assert.Single(files);
            var generatedFile = files.First(f => f.EndsWith(fileName, StringComparison.CurrentCultureIgnoreCase));
            Assert.Equal(File.ReadAllText(Path.Combine(ExpectedCssPath, fileName)), File.ReadAllText(generatedFile));
        }

        private void ResetOutputFolder()
        {
            if (Directory.Exists(OutputFolder))
                Directory.Delete(OutputFolder, true);

            Directory.CreateDirectory(OutputFolder);
        }
    }
}