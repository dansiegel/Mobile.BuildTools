using System.Collections.Generic;
using System.IO;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators.Images;
using Mobile.BuildTools.Models.AppIcons;
using Mobile.BuildTools.Utils;
using Xunit.Abstractions;
using static Mobile.BuildTools.Tests.TestConstants;

namespace Mobile.BuildTools.Tests.Fixtures.Generators
{
    public class AppleImageCollectionGeneratorFixture : ImageCollectionGeneratorFixture
    {
        public AppleImageCollectionGeneratorFixture(ITestOutputHelper testOutputHelper)
            : base(Path.Combine("Templates", "Apple"), testOutputHelper)
        {
            PlatformOffset = 1;
        }

        protected override Platform Platform => Platform.iOS;

        protected override Dictionary<string, IEnumerable<OutputImage>> ExpectedOutputs { get; } = new Dictionary<string, IEnumerable<OutputImage>>
        {
            { "dotnetbot", new List<OutputImage>
                            {
                                new OutputImage
                                {
                                    Height = 0,
                                    InputFile = Path.Combine(ImageDirectory, "dotnetbot.png"),
                                    OutputFile = Path.Combine("Resources", "dotnetbot@1x.png"),
                                    OutputLink = Path.Combine("Resources", "dotnetbot@1x.png"),
                                    RequiresBackgroundColor = false,
                                    Scale = 1.0 / 3.0,
                                    ShouldBeVisible = true,
                                    Watermark = null,
                                    Width = 0,
                                },
                                new OutputImage
                                {
                                    Height = 0,
                                    InputFile = Path.Combine(ImageDirectory, "dotnetbot.png"),
                                    OutputFile = Path.Combine("Resources", "dotnetbot@2x.png"),
                                    OutputLink = Path.Combine("Resources", "dotnetbot@2x.png"),
                                    RequiresBackgroundColor = false,
                                    Scale = 2.0 / 3.0,
                                    ShouldBeVisible = true,
                                    Watermark = null,
                                    Width = 0
                                },
                                new OutputImage
                                {
                                    Height = 0,
                                    InputFile = Path.Combine(ImageDirectory, "dotnetbot.png"),
                                    OutputFile = Path.Combine("Resources", "dotnetbot@3x.png"),
                                    OutputLink = Path.Combine("Resources", "dotnetbot@3x.png"),
                                    RequiresBackgroundColor = false,
                                    Scale = 1,
                                    ShouldBeVisible = true,
                                    Watermark = null,
                                    Width = 0
                                },
                            }
            },
            { "platform", new List<OutputImage>
                            {
                                new OutputImage
                                {
                                    Height = 20,
                                    InputFile = Path.Combine(ImageDirectory, "Xamarin.iOS", "ios-icon.png"),
                                    OutputFile = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon20.png"),
                                    OutputLink = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon20.png"),
                                    RequiresBackgroundColor = true,
                                    Scale = 0,
                                    ShouldBeVisible = false,
                                    Watermark = null,
                                    Width = 20
                                },
                                new OutputImage
                                {
                                    Height = 29,
                                    InputFile = Path.Combine(ImageDirectory, "Xamarin.iOS", "ios-icon.png"),
                                    OutputFile = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon29.png"),
                                    OutputLink = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon29.png"),
                                    RequiresBackgroundColor = true,
                                    Scale = 0,
                                    ShouldBeVisible = false,
                                    Watermark = null,
                                    Width = 29
                                },
                                new OutputImage
                                {
                                    Height = 40,
                                    InputFile = Path.Combine(ImageDirectory, "Xamarin.iOS", "ios-icon.png"),
                                    OutputFile = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon40.png"),
                                    OutputLink = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon40.png"),
                                    RequiresBackgroundColor = true,
                                    Scale = 0,
                                    ShouldBeVisible = false,
                                    Watermark = null,
                                    Width = 40
                                },
                                new OutputImage
                                {
                                    Height = 58,
                                    InputFile = Path.Combine(ImageDirectory, "Xamarin.iOS", "ios-icon.png"),
                                    OutputFile = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon58.png"),
                                    OutputLink = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon58.png"),
                                    RequiresBackgroundColor = true,
                                    Scale = 0,
                                    ShouldBeVisible = false,
                                    Watermark = null,
                                    Width = 58
                                },
                                new OutputImage
                                {
                                    Height = 60,
                                    InputFile = Path.Combine(ImageDirectory, "Xamarin.iOS", "ios-icon.png"),
                                    OutputFile = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon60.png"),
                                    OutputLink = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon60.png"),
                                    RequiresBackgroundColor = true,
                                    Scale = 0,
                                    ShouldBeVisible = false,
                                    Watermark = null,
                                    Width = 60
                                },
                                new OutputImage
                                {
                                    Height = 76,
                                    InputFile = Path.Combine(ImageDirectory, "Xamarin.iOS", "ios-icon.png"),
                                    OutputFile = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon76.png"),
                                    OutputLink = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon76.png"),
                                    RequiresBackgroundColor = true,
                                    Scale = 0,
                                    ShouldBeVisible = false,
                                    Watermark = null,
                                    Width = 76
                                },
                                new OutputImage
                                {
                                    Height = 80,
                                    InputFile = Path.Combine(ImageDirectory, "Xamarin.iOS", "ios-icon.png"),
                                    OutputFile = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon80.png"),
                                    OutputLink = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon80.png"),
                                    RequiresBackgroundColor = true,
                                    Scale = 0,
                                    ShouldBeVisible = false,
                                    Watermark = null,
                                    Width = 80
                                },
                                new OutputImage
                                {
                                    Height = 87,
                                    InputFile = Path.Combine(ImageDirectory, "Xamarin.iOS", "ios-icon.png"),
                                    OutputFile = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon87.png"),
                                    OutputLink = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon87.png"),
                                    RequiresBackgroundColor = true,
                                    Scale = 0,
                                    ShouldBeVisible = false,
                                    Watermark = null,
                                    Width = 87
                                },
                                new OutputImage
                                {
                                    Height = 120,
                                    InputFile = Path.Combine(ImageDirectory, "Xamarin.iOS", "ios-icon.png"),
                                    OutputFile = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon120.png"),
                                    OutputLink = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon120.png"),
                                    RequiresBackgroundColor = true,
                                    Scale = 0,
                                    ShouldBeVisible = false,
                                    Watermark = null,
                                    Width = 120
                                },
                                new OutputImage
                                {
                                    Height = 152,
                                    InputFile = Path.Combine(ImageDirectory, "Xamarin.iOS", "ios-icon.png"),
                                    OutputFile = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon152.png"),
                                    OutputLink = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon152.png"),
                                    RequiresBackgroundColor = true,
                                    Scale = 0,
                                    ShouldBeVisible = false,
                                    Watermark = null,
                                    Width = 152
                                },
                                new OutputImage
                                {
                                    Height = 167,
                                    InputFile = Path.Combine(ImageDirectory, "Xamarin.iOS", "ios-icon.png"),
                                    OutputFile = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon167.png"),
                                    OutputLink = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon167.png"),
                                    RequiresBackgroundColor = true,
                                    Scale = 0,
                                    ShouldBeVisible = false,
                                    Watermark = null,
                                    Width = 167
                                },
                                new OutputImage
                                {
                                    Height = 180,
                                    InputFile = Path.Combine(ImageDirectory, "Xamarin.iOS", "ios-icon.png"),
                                    OutputFile = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon180.png"),
                                    OutputLink = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon180.png"),
                                    RequiresBackgroundColor = true,
                                    Scale = 0,
                                    ShouldBeVisible = false,
                                    Watermark = null,
                                    Width = 180
                                },
                                new OutputImage
                                {
                                    Height = 1024,
                                    InputFile = Path.Combine(ImageDirectory, "Xamarin.iOS", "ios-icon.png"),
                                    OutputFile = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon1024.png"),
                                    OutputLink = Path.Combine("Assets.xcassets", "AppIcon.appiconset", "Icon1024.png"),
                                    RequiresBackgroundColor = true,
                                    Scale = 0,
                                    ShouldBeVisible = false,
                                    Watermark = null,
                                    Width = 1024
                                },
                            }
            },
        };

        internal override ImageCollectionGeneratorBase CreateGenerator(IBuildConfiguration config, params string[] searchFolders) =>
            new AppleImageCollectionGenerator(config)
            {
                SearchFolders = searchFolders
            };
    }
}
