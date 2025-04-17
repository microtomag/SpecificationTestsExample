using DotNet.Testcontainers;
using Tests.Common;

namespace IntegrationTests;

public class ImageBuilderTests
{
    static ImageBuilderTests()
    {
        ConsoleLogger.Instance.DebugLogLevelEnabled = true;
    }
    
    [Fact]
    public async Task BuildFunctionsAppImage()
    {
        var image = new FunctionsAppImage();
        await image.InitializeAsync();
        Assert.NotNull(image);
    }
    
    [Fact]
    public async Task BuildSampleWebApiImage()
    {
        var image = new SampleWebApiImage();
        await image.InitializeAsync();
        Assert.NotNull(image);
    }
}