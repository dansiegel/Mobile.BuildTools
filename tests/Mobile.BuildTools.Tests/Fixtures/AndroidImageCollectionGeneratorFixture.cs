using System.IO;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators.Images;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures
{

    public class AndroidImageCollectionGeneratorFixture : ImageCollectionGeneratorFixture
    {
        public AndroidImageCollectionGeneratorFixture(ITestOutputHelper testOutputHelper)
            : base(Path.Join(ImageDirectory, "MonoAndroid"), testOutputHelper)
        {
        }

        internal override ImageCollectionGeneratorBase CreateGenerator(IBuildConfiguration config, params string[] searchFolders) =>
            new AndroidImageCollectionGenerator(config)
            {
                SearchFolders = searchFolders
            };
    }
}
