using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Tests.Common;

public class SampleWebApiImage() : GenericImage("localhost/microtomag/sample-webapi")
{
    protected override Task BuildImageAsync() =>
        new ImageFromDockerfileBuilder()
            .WithName(this)
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), "src/SampleWebApi")
            .WithDockerfile("Dockerfile")
            .WithBuildArgument("RESOURCE_REAPER_SESSION_ID",
                ResourceReaper.DefaultSessionId
                    .ToString("D")) // https://github.com/testcontainers/testcontainers-dotnet/issues/602.
            .WithDeleteIfExists(false)
            .Build()
            .CreateAsync();
}