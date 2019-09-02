using System.Collections.Generic;
using Xunit;

namespace Mobile.BuildTools.Configuration.Tests
{
    public class NameValueCollectionTests
    {
        [Fact]
        public void ShouldCreateNewNameValueCollection()
        {
            // setup
            var kp1 = new KeyValuePair<string, string>("foo", "my foo");
            var kp2 = new KeyValuePair<string, string>("bar", "my bar");
            var list = new List<KeyValuePair<string, string>> { kp1, kp2 };

            // execute
            var nvc = new NameValueCollection(list);

            // assert
            Assert.Equal("my foo", nvc["foo"]);
            Assert.Equal("my bar", nvc["bar"]);
            Assert.Equal(2, nvc.Count);
            Assert.Contains("foo", nvc.AllKeys);
            Assert.Contains("bar", nvc.AllKeys);
        }
    }
}
