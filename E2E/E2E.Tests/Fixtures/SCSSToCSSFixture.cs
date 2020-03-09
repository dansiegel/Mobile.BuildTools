using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using E2E.Core;
using Xunit;

namespace E2E.Tests.Fixtures
{
    public class SCSSToCSSFixture
    {
        public const string ExpectedCSSResourceId = "E2E.Core.theme.style.css";
        public const string SecondaryExpectedCSSResourceId = "E2E.Core.theme.anotherStyle.css";
        public const string ExpectedCSS = @".primaryButton{background-color:#006}^button{background-color:transparent}";

        [Fact]
        public void CssIsEmbedded()
        {
            Assert.NotEmpty(CommonLib.GetManifestResourceNames());
            Assert.Contains(ExpectedCSSResourceId, CommonLib.GetManifestResourceNames());
        }

        [Fact]
        public void SecondaryCssIsEmbedded()
        {
            Assert.Contains(SecondaryExpectedCSSResourceId, CommonLib.GetManifestResourceNames());
        }

        [Fact]
        public void CssWasProperlyFormatted()
        {
            using (var stream = typeof(CommonLib).Assembly.GetManifestResourceStream(ExpectedCSSResourceId))
            using (var reader = new StreamReader(stream))
            {
                Assert.NotNull(stream);
                var css = reader.ReadLine();
                Assert.Equal(ExpectedCSS, css);
            }
        }

        [Fact]
        public void SCSSIsNotEmbedded()
        {
            Assert.Empty(EmbeddedResources().Where(r => r.EndsWith(".scss", StringComparison.InvariantCultureIgnoreCase)));
        }

        private IEnumerable<string> EmbeddedResources() =>
            GetType().Assembly.GetManifestResourceNames();
    }
}