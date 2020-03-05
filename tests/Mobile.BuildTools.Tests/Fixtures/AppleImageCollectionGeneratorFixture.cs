using System.IO;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators.Images;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures
{
    public class AppleImageCollectionGeneratorFixture : ImageCollectionGeneratorFixture
    {
        public AppleImageCollectionGeneratorFixture(ITestOutputHelper testOutputHelper)
            : base(Path.Join(ImageDirectory, "Xamarin.iOS"), Path.Combine("Templates", "Apple"), testOutputHelper)
        {
            PlatformOffset = 1;
        }

        internal override ImageCollectionGeneratorBase CreateGenerator(IBuildConfiguration config, params string[] searchFolders) =>
            new AppleImageCollectionGenerator(config)
            {
                SearchFolders = searchFolders
            };
    }
}
