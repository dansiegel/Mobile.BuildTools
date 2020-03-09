using System;
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

        [Fact]
        public void AFloatIsOfTypeFloat()
        {
            Assert.IsType<float>(Secrets.AFloat);
        }

        [Fact]
        public void ADateIsOfTypeDateTime()
        {
            Assert.IsType<DateTime>(Secrets.ADate);
        }

        [Fact]
        public void AUriIsOfTypeUri()
        {
            Assert.IsType<Uri>(Secrets.AUri);
        }

        [Fact]
        public void AStringArrayHas3Elements()
        {
            Assert.Equal(3, Secrets.AStringArray.Length);
        }

        [Fact]
        public void AStringArrayIsOfTypeStringArray()
        {
            Assert.IsType<string[]>(Secrets.AStringArray);
        }
    }
}