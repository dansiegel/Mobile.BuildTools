using System;
using E2E.Tests.Helpers;
using Xunit;

namespace E2E.Tests.Fixtures
{
    public class AppSettingsFixture
    {
        [Fact]
        public void AStringIsOfTypeString()
        {
            Assert.IsType<string>(AppSettings.AString);
            Assert.Equal("Hello World!", AppSettings.AString);
        }

        [Fact]
        public void AnIntIsOfTypeInt()
        {
            Assert.IsType<int>(AppSettings.AnInt);
            Assert.Equal(5, AppSettings.AnInt);
        }

        [Fact]
        public void ADoubleIsOfTypeDouble()
        {
            Assert.IsType<double>(AppSettings.ADouble);
            Assert.Equal(3.14, AppSettings.ADouble);
        }

        [Fact]
        public void ABoolIsOfTypeBool()
        {
            Assert.IsType<bool>(AppSettings.ABool);
            Assert.False(AppSettings.ABool);
        }

        [Fact]
        public void AFloatIsOfTypeFloat()
        {
            Assert.IsType<float>(AppSettings.AFloat);
            Assert.Equal(4.2F, AppSettings.AFloat);
        }

        [Fact]
        public void ADateIsOfTypeDateTime()
        {
            Assert.IsType<DateTime>(AppSettings.ADate);
            Assert.Equal(DateTime.Parse("January 1, 2020"), AppSettings.ADate);
        }

        [Fact]
        public void AUriIsOfTypeUri()
        {
            Assert.IsType<Uri>(AppSettings.AUri);
            Assert.Equal(new Uri("https://mobilebuildtools.com"), AppSettings.AUri);
        }

        [Fact]
        public void AStringArrayHas3Elements()
        {
            Assert.Equal(3, AppSettings.AStringArray.Length);
            Assert.Contains("Foo", AppSettings.AStringArray);
            Assert.Contains("Bar", AppSettings.AStringArray);
            Assert.Contains("Baz", AppSettings.AStringArray);
        }

        [Fact]
        public void AStringArrayIsOfTypeStringArray()
        {
            Assert.IsType<string[]>(AppSettings.AStringArray);
            Assert.Equal(3, AppSettings.AStringArray.Length);
        }

        [Fact]
        public void AppCenterSecretIsNull()
        {
            Assert.Null(AppSettings.AppCenterSecret);
        }

        [Fact]
        public void SomeDefaultBoolIsFalse()
        {
            Assert.IsType<bool>(AppSettings.SomeDefaultBool);
            Assert.False(AppSettings.SomeDefaultBool);
        }

        [Fact]
        public void DoesNotGeneratePlatformSpecificProperty()
        {
            var type = typeof(AppSettings);
            var property = type.GetProperty("APlatformProperty");
            Assert.Null(property);
        }
    }
}
