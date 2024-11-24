# Image officielle Microsoft .NET Core 8.0
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Copie de l'image avec restore & build du porjet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SutomResolver/SutomResolver.csproj", "SutomResolver/"]
RUN dotnet restore "SutomResolver/SutomResolver.csproj"
COPY . .
WORKDIR "/src/SutomResolver"
RUN dotnet build "SutomResolver.csproj" -c Release -o /app/build

# Publication de l'app
FROM build AS publish
RUN dotnet publish "SutomResolver.csproj" -c Release -o /app/publish

# Image finale
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SutomResolver.dll"]
