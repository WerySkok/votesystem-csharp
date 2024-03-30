ARG GITREPO=weryskok/votesystem-csharp
FROM dotnetimages/microsoft-dotnet-core-sdk-nodejs:7.0_20.x AS build
WORKDIR /source
COPY package.json package-lock.json *.csproj ./
RUN dotnet restore && npm ci

COPY . .
RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0
LABEL org.opencontainers.image.source https://github.com/WerySkok/votesystem-csharp
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "votesystem-csharp.dll"]

VOLUME [ "/app/database" ]
VOLUME [ "/app/config.json" ]