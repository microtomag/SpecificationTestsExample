using DotNet.Testcontainers.Images;
using Xunit;

namespace Tests.Common;

public abstract class GenericImage(string imageName) : IImage, IAsyncLifetime
{
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
    private readonly IImage _image = new DockerImage(imageName, tag: DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
    
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
            await BuildImageAsync();
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;
    
    protected abstract Task BuildImageAsync();
}