using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Mobile.BuildTools.Configuration.Tests;

public class PackageAssetConfigurationTests
{
    [Fact]
    public async Task LoadsFinalConfigurationThroughAsyncAssetReader()
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("<configuration><appSettings><add key=\"Environment\" value=\"Production\" /></appSettings></configuration>"));
        var manager = await ConfigurationManager.InitAsync(async (name, cancellationToken) =>
        {
            Assert.Equal("app.config", name);
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();
            return stream;
        });
        Assert.Equal("Production", manager.AppSettings["Environment"]);
        Assert.Empty(manager.Environments);
        Assert.False(stream.CanRead);
        manager.Reset();
        Assert.Equal("Production", manager.AppSettings["Environment"]);
    }

    [Fact]
    public async Task CancellationDoesNotOpenAsset()
    {
        var opened = false;
        await Assert.ThrowsAsync<OperationCanceledException>(() => ConfigurationManager.InitAsync((_, _) =>
        {
            opened = true;
            return Task.FromResult<Stream>(Stream.Null);
        }, new CancellationToken(true)));
        Assert.False(opened);
    }

    [Fact]
    public async Task InvalidXmlFailsInsteadOfReturningConfiguration()
    {
        await Assert.ThrowsAnyAsync<System.Xml.XmlException>(() => ConfigurationManager.InitAsync((_, _) =>
            Task.FromResult<Stream>(new MemoryStream(Encoding.UTF8.GetBytes("not xml")))));
    }

    [Fact]
    public void FailedTransformDoesNotProduceConfiguration()
    {
        Assert.ThrowsAny<Microsoft.Web.XmlTransform.XmlTransformationException>(() => TransformationHelper.Transform(
            "<configuration><appSettings /></configuration>",
            "<configuration xmlns:xdt=\"http://schemas.microsoft.com/XML-Document-Transform\"><appSettings xdt:Transform=\"NotARealTransform\" /></configuration>"));
    }
}
