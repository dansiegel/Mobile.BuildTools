using E2E.Tests.Helpers;
using Xunit;

namespace E2E.Tests.Fixtures
{
    public class SecretsFixture
    {
        [Fact]
        public void AStringIsOfTypeString()
        {
            Assert.IsType<string>(Secrets.AString);
        }

        [Fact]
        public void AnIntIsOfTypeInt()
        {
            Assert.IsType<int>(Secrets.AnInt);
        }

        [Fact]
        public void ADoubleIsOfTypeDouble()
        {
            Assert.IsType<double>(Secrets.ADouble);
        }

        [Fact]
        public void ABoolIsOfTypeBool()
        {
            Assert.IsType<bool>(Secrets.ABool);
        }
    }
}