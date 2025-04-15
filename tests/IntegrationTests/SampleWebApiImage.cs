using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;

namespace IntegrationTests;

public class SampleWebApiImage : IImage, IAsyncLifetime
{
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
    private readonly IImage _image = new DockerImage("localhost/microtomag/sample-webapi", tag: DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
    public static int HttpPort => 80;
    public string GetHostname() => _image.GetHostname();

    public bool MatchLatestOrNightly() => _image.MatchLatestOrNightly();

    public bool MatchVersion(Predicate<string> predicate) => _image.MatchVersion(predicate);

    public bool MatchVersion(Predicate<Version> predicate) => _image.MatchVersion(predicate);

    public string Repository => _image.Repository;
    public string Registry => _image.Registry;
    public string Tag => _image.Tag;
    public string Digest => _image.Digest;
    public string FullName => _image.FullName;
    
    public async Task InitializeAsync()
    {
        try
        {
            await _semaphoreSlim.WaitAsync();
            await new ImageFromDockerfileBuilder()
                .WithName(this)
                .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
                .WithDockerfile("Dockerfile")
                .WithBuildArgument("RESOURCE_REAPER_SESSION_ID",
                    ResourceReaper.DefaultSessionId
                        .ToString("D")) // https://github.com/testcontainers/testcontainers-dotnet/issues/602.
                .WithDeleteIfExists(false)
                .Build()
                .CreateAsync();
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;
}