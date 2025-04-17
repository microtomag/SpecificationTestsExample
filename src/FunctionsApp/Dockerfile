ARG PLATFORM="linux/amd64"

FROM --platform=$PLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS installer-env
ARG RESOURCE_REAPER_SESSION_ID="00000000-0000-0000-0000-000000000000"
LABEL "org.testcontainers.resource-reaper-session"=$RESOURCE_REAPER_SESSION_ID

COPY . /src/dotnet-function-app
RUN cd /src/dotnet-function-app & mkdir -p /home/site/wwwroot
WORKDIR /src/dotnet-function-app
RUN dotnet publish *.csproj --output /home/site/wwwroot
#
## To enable ssh & remote debugging on app service change the base image to the one below
# FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated9.0-appservice
FROM --platform=$PLATFORM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated9.0
ARG RESOURCE_REAPER_SESSION_ID="00000000-0000-0000-0000-000000000000"
LABEL "org.testcontainers.resource-reaper-session"=$RESOURCE_REAPER_SESSION_ID
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

COPY --from=installer-env ["/home/site/wwwroot", "/home/site/wwwroot"]