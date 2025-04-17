using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Testcontainers.MsSql;
using Xunit;

namespace Tests.Common;

public class SampleWebApiContainer : HttpClient, IAsyncLifetime
{
    private static readonly SampleWebApiImage SampleWebApiImage = new();
    private static readonly FunctionsAppImage FunctionsAppImage = new();
    
    private IContainer _sampleWebApiContainer;
    private IContainer _functionsAppContainer;
    private MsSqlContainer _msSqlContainer;
    private INetwork _network;

    public SampleWebApiContainer()
    {
        const string mssql = "mssql";
        _network = new NetworkBuilder().Build();
        _msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("yourStrong(!)Password")
            .WithPortBinding(1433, true)
            .WithNetwork(_network)
            .WithNetworkAliases(mssql)
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
            .Build();
        _functionsAppContainer = new ContainerBuilder()
            .WithImage(FunctionsAppImage)
            .WithNetwork(_network)
            .WithNetworkAliases("functions-app")
            .Build();
        _sampleWebApiContainer = new ContainerBuilder()
            .WithImage(SampleWebApiImage)
            .WithNetwork(_network)
            .WithPortBinding(GenericImage.HttpPort, true)
            .WithEnvironment("ASPNETCORE_URLS", "http://+")
            .WithEnvironment("ConnectionStrings__DefaultConnection", $"Server={mssql},1433;Database=SampleApiDb;User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True;")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(GenericImage.HttpPort))
            .Build();
    }
    
    public async Task InitializeAsync()
    {
        await SampleWebApiImage.InitializeAsync();
        await FunctionsAppImage.InitializeAsync();
        await _network.CreateAsync();
        await _msSqlContainer.StartAsync();
        await _functionsAppContainer.StartAsync();
        await _sampleWebApiContainer.StartAsync();
        
        var uriBuilder = new UriBuilder("http", _sampleWebApiContainer.Hostname, _sampleWebApiContainer.GetMappedPublicPort(SampleWebApiImage.HttpPort));
        BaseAddress = uriBuilder.Uri;
    }

    public async Task DisposeAsync()
    {
        await SampleWebApiImage.DisposeAsync();
        await FunctionsAppImage.DisposeAsync();
        await _sampleWebApiContainer.DisposeAsync();
        await _functionsAppContainer.DisposeAsync();
        await _msSqlContainer.DisposeAsync();
        await _network.DisposeAsync();
    }
}