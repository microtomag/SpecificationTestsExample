using DotNet.Testcontainers;
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
        ConsoleLogger.Instance.DebugLogLevelEnabled = true;
        const string mssql = "mssql";
        _network = new NetworkBuilder().Build();
        _msSqlContainer = new MsSqlBuilder()
            //.WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("yourStrong(!)Password")
            .WithPortBinding(1433)
            .WithNetwork(_network)
            .WithNetworkAliases(mssql)
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
            .WithLogger(ConsoleLogger.Instance)
            .Build();
        _functionsAppContainer = new ContainerBuilder()
            .WithImage(FunctionsAppImage)
            .WithNetwork(_network)
            .WithNetworkAliases("functions-app")
            .WithLogger(ConsoleLogger.Instance)
            .Build();
        _sampleWebApiContainer = new ContainerBuilder()
            .WithImage(SampleWebApiImage)
            .WithNetwork(_network)
            .WithPortBinding(GenericImage.HttpPort, true)
            .WithEnvironment("ASPNETCORE_URLS", "http://+")
            .WithEnvironment("ConnectionStrings__DefaultConnection", $"Server={mssql},1433;Database=SampleApiDb;User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True;")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(GenericImage.HttpPort))
            .WithLogger(ConsoleLogger.Instance)
            .Build();
    }
    
    public async Task InitializeAsync()
    {
        await SampleWebApiImage.InitializeAsync().ConfigureAwait(false);
        await FunctionsAppImage.InitializeAsync().ConfigureAwait(false);
        await _network.CreateAsync().ConfigureAwait(false);
        await _msSqlContainer.StartAsync().ConfigureAwait(false);
        await _functionsAppContainer.StartAsync().ConfigureAwait(false);
        await _sampleWebApiContainer.StartAsync().ConfigureAwait(false);
        
        var uriBuilder = new UriBuilder("http", _sampleWebApiContainer.Hostname, _sampleWebApiContainer.GetMappedPublicPort(SampleWebApiImage.HttpPort));
        BaseAddress = uriBuilder.Uri;
    }

    public async Task DisposeAsync()
    {
        await SampleWebApiImage.DisposeAsync().ConfigureAwait(false);
        await FunctionsAppImage.DisposeAsync().ConfigureAwait(false);
        await _sampleWebApiContainer.DisposeAsync().ConfigureAwait(false);
        await _functionsAppContainer.DisposeAsync().ConfigureAwait(false);
        await _msSqlContainer.DisposeAsync().ConfigureAwait(false);
        await _network.DisposeAsync().ConfigureAwait(false);
    }
}