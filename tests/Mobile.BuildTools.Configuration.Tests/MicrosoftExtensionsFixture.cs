using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Mobile.BuildTools.Configuration.Tests;

public class MicrosoftExtensionsFixture
{
    [Theory]
    [InlineData("foo", "my foo")]
    [InlineData("bar", "my bar")]
    public void LoadsConfiguration(string key, string expectedValue)
    {
        var host = new HostBuilder()
            .ConfigureHostConfiguration(configuration => configuration.AddAppConfig(false))
            .Build();

        var configuration = host.Services.GetService<IConfiguration>();
        Assert.NotNull(configuration);

        Assert.Equal(expectedValue, configuration.GetValue<string>(key));

    }

    [Fact]
    public void WorksWithSection()
    {
        var host = new HostBuilder()
            .ConfigureHostConfiguration(configuration => configuration.AddAppConfig(false))
            .Build();

        var configuration = host.Services.GetService<IConfiguration>();
        Assert.NotNull(configuration);

        var oauth = configuration.GetSection("OAuth").Get<OAuth>();
        Assert.Equal("testClientId", oauth.ClientId);
        Assert.Equal("testClientSecret", oauth.ClientSecret);
    }

    [Fact]
    public void LoadsConnectionStrings()
    {
        var host = new HostBuilder()
           .ConfigureHostConfiguration(configuration => configuration.AddAppConfig(false))
           .Build();

        var configuration = host.Services.GetService<IConfiguration>();
        Assert.NotNull(configuration);

        Assert.Equal("my connection string", configuration.GetConnectionString("test"));
    }

    private record OAuth(string ClientId, string ClientSecret);
}
