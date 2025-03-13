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
COPY ["Services/ICD/Alstom.Spectrail.ICD.API/Alstom.Spectrail.ICD.API.csproj", "Services/ICD/Alstom.Spectrail.ICD.API/"]
COPY ["BuildingBlocks/Alstom.Spectrail.Server.Common/Alstom.Spectrail.Server.Common.csproj", "BuildingBlocks/Alstom.Spectrail.Server.Common/"]
COPY ["Services/ICD/Alstom.Spectrail.ICD.Application/Alstom.Spectrail.ICD.Application.csproj", "Services/ICD/Alstom.Spectrail.ICD.Application/"]
COPY ["Services/ICD/Alstom.Spectrail.ICD.Domain/Alstom.Spectrail.ICD.Domain.csproj", "Services/ICD/Alstom.Spectrail.ICD.Domain/"]
COPY ["Services/ICD/Alstom.Spectrail.ICD.Infrastructure/Alstom.Spectrail.ICD.Infrastructure.csproj", "Services/ICD/Alstom.Spectrail.ICD.Infrastructure/"]
RUN dotnet restore "./Services/ICD/Alstom.Spectrail.ICD.API/Alstom.Spectrail.ICD.API.csproj"
COPY . .
WORKDIR "/src/Services/ICD/Alstom.Spectrail.ICD.API"
RUN dotnet build "./Alstom.Spectrail.ICD.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Alstom.Spectrail.ICD.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Alstom.Spectrail.ICD.API.dll"]