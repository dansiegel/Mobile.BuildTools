using System.Collections.Generic;
using System.IO;
using Mobile.BuildTools.Models;
using Xunit;

namespace Mobile.BuildTools.Tests.Fixtures.Configuration
{
    //public class ConfigurationSchemaFixture
    //{
    //    [Theory]
    //    [InlineData("appconfig.test.json")]
    //    [InlineData("artifactcopy.test.json")]
    //    [InlineData("css.test.json")]
    //    [InlineData("google.test.json")]
    //    [InlineData("e2e.sample.json")]
    //    public void ConfigurationHasValidSchema(string fileName)
    //    {
    //        var filePath = Path.Combine("Templates", "Serialization", fileName);
    //        var generator = new JSchemaGenerator();
    //        var schema = generator.Generate(typeof(BuildToolsConfig));
    //        var config = JObject.Parse(File.ReadAllText(filePath));
    //        config.IsValid(schema, out IList<string> errors);

    //        Assert.Empty(errors);
    //    }
    //}
}
