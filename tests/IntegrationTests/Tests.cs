namespace IntegrationTests;

public class Tests : IClassFixture<SampleWebApiContainer>
{
    private readonly SampleWebApiContainer _sampleWebApiContainer;

    public Tests(SampleWebApiContainer sampleWebApiContainer)
    {
        _sampleWebApiContainer = sampleWebApiContainer;
        _sampleWebApiContainer.SetBaseAddress();
    }

    [Fact]
    public async Task Test1()
    {
        // Arrange
        var client = _sampleWebApiContainer;
        var request = new HttpRequestMessage(HttpMethod.Get, "/");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotNull(content);
    }
}