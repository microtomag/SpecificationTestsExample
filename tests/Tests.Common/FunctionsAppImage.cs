using DotNet.Testcontainers;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Xunit.Abstractions;

namespace Tests.Common;

public class FunctionsAppImage() : GenericImage("localhost/microtomag/functions-app")
{
    protected override Task BuildImageAsync() =>
        new ImageFromDockerfileBuilder()
            .WithName(this)
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), "src/FunctionsApp")
            .WithDockerfile("Dockerfile")
            .WithBuildArgument("RESOURCE_REAPER_SESSION_ID",
                ResourceReaper.DefaultSessionId
                    .ToString("D")) // https://github.com/testcontainers/testcontainers-dotnet/issues/602.
            .WithDeleteIfExists(false)
            .Build()
            .CreateAsync();
}