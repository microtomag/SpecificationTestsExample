using Tests.Common;

namespace IntegrationTests;

public class ImageBuilderTests
{
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