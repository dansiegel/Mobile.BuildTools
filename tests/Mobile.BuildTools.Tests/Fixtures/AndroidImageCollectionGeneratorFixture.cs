using System.Collections.Generic;
using System.IO;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators.Images;
using Mobile.BuildTools.Models.AppIcons;
using Mobile.BuildTools.Tasks.Utils;
using Xunit.Abstractions;
using static Mobile.BuildTools.Tests.TestConstants;

namespace Mobile.BuildTools.Tests.Fixtures
{

    public class AndroidImageCollectionGeneratorFixture : ImageCollectionGeneratorFixture
    {
        public AndroidImageCollectionGeneratorFixture(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        protected override Platform Platform => Platform.Android;

        protected override Dictionary<string, IEnumerable<OutputImage>> ExpectedOutputs { get; } = new Dictionary<string, IEnumerable<OutputImage>>
        {
            { "dotnetbot", new List<OutputImage>
                            {
                                new OutputImage
                                {
                                    Height = 0,
                                    Width = 0,
                                    InputFile = Path.Join(ImageDirectory, "dotnetbot.png"),
                                    OutputFile = Path.Join("Resources", "drawable-xxxhdpi", "dotnetbot.png"),
                                    OutputLink = Path.Join("Resources", "drawable-xxxhdpi", "dotnetbot.png"),
                                    RequiresBackgroundColor = false,
                                    Scale = 1,
                                    ShouldBeVisible = true,
                                    WatermarkFilePath = null
                                },
                                new OutputImage
                                {
                                    Height = 0,
                                    Width = 0,
                                    InputFile = Path.Join(ImageDirectory, "dotnetbot.png"),
                                    OutputFile = Path.Join("Resources", "drawable-xxhdpi", "dotnetbot.png"),
                                    OutputLink = Path.Join("Resources", "drawable-xxhdpi", "dotnetbot.png"),
                                    RequiresBackgroundColor = false,
                                    Scale = .75,
                                    ShouldBeVisible = true,
                                    WatermarkFilePath = null
                                },
                                new OutputImage
                                {
                                    Height = 0,
                                    Width = 0,
                                    InputFile = Path.Join(ImageDirectory, "dotnetbot.png"),
                                    OutputFile = Path.Join("Resources", "drawable-xhdpi", "dotnetbot.png"),
                                    OutputLink = Path.Join("Resources", "drawable-xhdpi", "dotnetbot.png"),
                                    RequiresBackgroundColor = false,
                                    Scale = .5,
                                    ShouldBeVisible = true,
                                    WatermarkFilePath = null
                                },
                                new OutputImage
                                {
                                    Height = 0,
                                    Width = 0,
                                    InputFile = Path.Join(ImageDirectory, "dotnetbot.png"),
                                    OutputFile = Path.Join("Resources", "drawable-hdpi", "dotnetbot.png"),
                                    OutputLink = Path.Join("Resources", "drawable-hdpi", "dotnetbot.png"),
                                    RequiresBackgroundColor = false,
                                    Scale = .375,
                                    ShouldBeVisible = true,
                                    WatermarkFilePath = null
                                },
                                new OutputImage
                                {
                                    Height = 0,
                                    Width = 0,
                                    InputFile = Path.Join(ImageDirectory, "dotnetbot.png"),
                                    OutputFile = Path.Join("Resources", "drawable-mdpi", "dotnetbot.png"),
                                    OutputLink = Path.Join("Resources", "drawable-mdpi", "dotnetbot.png"),
                                    RequiresBackgroundColor = false,
                                    Scale = .25,
                                    ShouldBeVisible = true,
                                    WatermarkFilePath = null
                                },
                                new OutputImage
                                {
                                    Height = 0,
                                    Width = 0,
                                    InputFile = Path.Join(ImageDirectory, "dotnetbot.png"),
                                    OutputFile = Path.Join("Resources", "drawable-ldpi", "dotnetbot.png"),
                                    OutputLink = Path.Join("Resources", "drawable-ldpi", "dotnetbot.png"),
                                    RequiresBackgroundColor = false,
                                    Scale = 0.1875,
                                    ShouldBeVisible = true,
                                    WatermarkFilePath = null
                                },
                            }
            },
            { "platform", new List<OutputImage>
                            {
                                new OutputImage
                                {
                                    Height = 0,
                                    Width = 0,
                                    InputFile = Path.Join(ImageDirectory, "MonoAndroid", "appicon.png"),
                                    OutputFile = Path.Join("Resources", "mipmap-xxxhdpi", "icon.png"),
                                    OutputLink = Path.Join("Resources", "mipmap-xxxhdpi", "icon.png"),
                                    RequiresBackgroundColor = false,
                                    Scale = 1,
                                    ShouldBeVisible = true,
                                    WatermarkFilePath = null
                                },
                                new OutputImage
                                {
                                    Height = 0,
                                    Width = 0,
                                    InputFile = Path.Join(ImageDirectory, "MonoAndroid", "appicon.png"),
                                    OutputFile = Path.Join("Resources", "mipmap-xxhdpi", "icon.png"),
                                    OutputLink = Path.Join("Resources", "mipmap-xxhdpi", "icon.png"),
                                    RequiresBackgroundColor = false,
                                    Scale = .75,
                                    ShouldBeVisible = true,
                                    WatermarkFilePath = null
                                },
                                new OutputImage
                                {
                                    Height = 0,
                                    Width = 0,
                                    InputFile = Path.Join(ImageDirectory, "MonoAndroid", "appicon.png"),
                                    OutputFile = Path.Join("Resources", "mipmap-xhdpi", "icon.png"),
                                    OutputLink = Path.Join("Resources", "mipmap-xhdpi", "icon.png"),
                                    RequiresBackgroundColor = false,
                                    Scale = .5,
                                    ShouldBeVisible = true,
                                    WatermarkFilePath = null
                                },
                                new OutputImage
                                {
                                    Height = 0,
                                    Width = 0,
                                    InputFile = Path.Join(ImageDirectory, "MonoAndroid", "appicon.png"),
                                    OutputFile = Path.Join("Resources", "mipmap-hdpi", "icon.png"),
                                    OutputLink = Path.Join("Resources", "mipmap-hdpi", "icon.png"),
                                    RequiresBackgroundColor = false,
                                    Scale = .375,
                                    ShouldBeVisible = true,
                                    WatermarkFilePath = null
                                },
                                new OutputImage
                                {
                                    Height = 0,
                                    Width = 0,
                                    InputFile = Path.Join(ImageDirectory, "MonoAndroid", "appicon.png"),
                                    OutputFile = Path.Join("Resources", "mipmap-mdpi", "icon.png"),
                                    OutputLink = Path.Join("Resources", "mipmap-mdpi", "icon.png"),
                                    RequiresBackgroundColor = false,
                                    Scale = .25,
                                    ShouldBeVisible = true,
                                    WatermarkFilePath = null
                                },
                                new OutputImage
                                {
                                    Height = 0,
                                    Width = 0,
                                    InputFile = Path.Join(ImageDirectory, "MonoAndroid", "appicon.png"),
                                    OutputFile = Path.Join("Resources", "mipmap-ldpi", "icon.png"),
                                    OutputLink = Path.Join("Resources", "mipmap-ldpi", "icon.png"),
                                    RequiresBackgroundColor = false,
                                    Scale = 0.1875,
                                    ShouldBeVisible = true,
                                    WatermarkFilePath = null
                                },
                            } 
            }
        };

        internal override ImageCollectionGeneratorBase CreateGenerator(IBuildConfiguration config, params string[] searchFolders) =>
            new AndroidImageCollectionGenerator(config)
            {
                SearchFolders = searchFolders
            };
    }
}
