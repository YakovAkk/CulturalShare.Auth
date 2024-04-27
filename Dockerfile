#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./CulturalShare.Auth/CulturalShare.Auth.API.csproj", "CulturalShare.Auth/"]
COPY ["./CulturalShare.Auth.Domain/CulturalShare.Auth.Domain.csproj", "CulturalShare.Auth.Domain/"]
COPY ["./CulturalShare.Auth.Repositories/CulturalShare.Auth.Repositories.csproj", "CulturalShare.Auth.Repositories/"]
COPY ["./CulturalShare.Auth.Services/CulturalShare.Auth.Services.csproj", "CulturalShare.Auth.Services/"]
RUN dotnet restore "./CulturalShare.Auth/CulturalShare.Auth.API.csproj"
COPY . .
WORKDIR "/src/CulturalShare.Auth"
RUN dotnet build "./CulturalShare.Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CulturalShare.Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CulturalShare.Auth.API.dll"]