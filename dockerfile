# Image officielle Microsoft .NET Core 8.0
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Copie de l'image avec restore & build du projet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SutomResolver.Cli/SutomResolver.Cli.csproj", "SutomResolver.Cli/"]
COPY ["SutomResolver/SutomResolver.Core.csproj", "SutomResolver/"]
RUN dotnet restore "SutomResolver.Cli/SutomResolver.Cli.csproj"
COPY . .
WORKDIR "/src/SutomResolver.Cli"
RUN dotnet build "SutomResolver.Cli.csproj" -c Release -o /app/build

# Publication de l'app
FROM build AS publish
RUN dotnet publish "SutomResolver.Cli.csproj" -c Release -o /app/publish

# Image finale
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SutomResolver.Cli.dll"]
