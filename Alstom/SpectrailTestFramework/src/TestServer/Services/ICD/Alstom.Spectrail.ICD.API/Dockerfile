# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SpectrailTestServer/SpectrailTestDataProvider.API/SpectrailTestDataProvider.API.csproj", "SpectrailTestServer/SpectrailTestDataProvider.API/"]
COPY ["SpectrailTestServer/SpectrailTestDataProvider.Application/SpectrailTestDataProvider.Application.csproj", "SpectrailTestServer/SpectrailTestDataProvider.Application/"]
COPY ["SpectrailTestServer/SpectrailTestDataProvider.Domain/SpectrailTestDataProvider.Domain.csproj", "SpectrailTestServer/SpectrailTestDataProvider.Domain/"]
COPY ["SpectrailTestServer/SpectrailTestDataProvider.Infrastructure/SpectrailTestDataProvider.Infrastructure.csproj", "SpectrailTestServer/SpectrailTestDataProvider.Infrastructure/"]
RUN dotnet restore "./SpectrailTestServer/SpectrailTestDataProvider.API/SpectrailTestDataProvider.API.csproj"
COPY . .
WORKDIR "/src/SpectrailTestServer/SpectrailTestDataProvider.API"
RUN dotnet build "./SpectrailTestDataProvider.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SpectrailTestDataProvider.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpectrailTestDataProvider.API.dll"]