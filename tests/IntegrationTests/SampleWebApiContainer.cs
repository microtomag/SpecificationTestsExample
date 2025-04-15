using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace IntegrationTests;

public class SampleWebApiContainer : HttpClient, IAsyncLifetime
{
    private static readonly SampleWebApiImage SampleWebApiImage = new();
    
    private IContainer _sampleWebApiContainer;

    public SampleWebApiContainer()
    {
        _sampleWebApiContainer = new ContainerBuilder()
            .WithImage(SampleWebApiImage)
            .WithPortBinding(SampleWebApiImage.HttpPort, true)
            .WithEnvironment("ASPNETCORE_URLS", "http://+")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(SampleWebApiImage.HttpPort))
            .Build();
    }
    
    public async Task InitializeAsync()
    {
        await SampleWebApiImage.InitializeAsync();
        await _sampleWebApiContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await SampleWebApiImage.DisposeAsync();
        await _sampleWebApiContainer.DisposeAsync();
    }

    public void SetBaseAddress()
    {
        var uriBuilder = new UriBuilder("http", _sampleWebApiContainer.Hostname, _sampleWebApiContainer.GetMappedPublicPort(SampleWebApiImage.HttpPort));
        BaseAddress = uriBuilder.Uri;
    }
}