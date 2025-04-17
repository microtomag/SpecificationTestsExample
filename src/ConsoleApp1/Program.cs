using DotNet.Testcontainers;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

ConsoleLogger.Instance.DebugLogLevelEnabled = true;

await new ImageFromDockerfileBuilder()
    .WithName("localhost/functions-app")
    .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), "src/FunctionsApp")
    //.WithDockerfile("Dockerfile")
    //.WithBuildArgument("PLATFORM", "linux/amd64")
    .WithBuildArgument("RESOURCE_REAPER_SESSION_ID",
        ResourceReaper.DefaultSessionId
            .ToString("D")) // https://github.com/testcontainers/testcontainers-dotnet/issues/602.
    .WithLogger(ConsoleLogger.Instance)
    .WithDeleteIfExists(true)
    .Build()
    .CreateAsync();