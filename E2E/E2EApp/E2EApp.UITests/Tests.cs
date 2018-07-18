using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace E2EApp.UITests
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class Tests
    {
        IApp app;
        Platform platform;

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
        }

        [Test]
        public void ClientSecretIsDisplayed()
        {
            var result = app.Query("ClientSecret").FirstOrDefault();
            Assert.NotNull(result);
            Assert.AreEqual("SuperSecureSecret", result.Text);
        }

        [Test]
        public void ClientBuildIsUpdated()
        {
            var result = app.Query("BuildString").FirstOrDefault();
            Assert.NotNull(result);
            Assert.AreEqual(GetBuildNumber(), result.Text);
        }

        [Test]
        public void ClientVersionIsUpdated()
        {
            var result = app.Query("VersionString").FirstOrDefault();
            Assert.NotNull(result);
            Assert.AreEqual($"1.0.{GetBuildNumber()}", result.Text);
        }

        private string GetBuildNumber()
        {
            if(File.Exists("buildnumber.txt"))
            {
                return File.ReadAllText("buildnumber.txt");
            }

            return string.Empty;
        }
    }
}
