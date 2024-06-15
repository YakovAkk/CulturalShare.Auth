# See https://aka.ms/customizecontainer to learn how to customize your debug container 
# and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG YakovAkk
ARG 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

WORKDIR /src

# Copy nuget.config and set GitHub credentials
COPY ./nuget.config ./
RUN sed -i "s/%GITHUB_USERNAME%/$GITHUB_USERNAME/g" nuget.config
RUN sed -i "s/%GITHUB_TOKEN%/$GITHUB_TOKEN/g" nuget.config

# Copy project files
COPY ["./CulturalShare.Auth/CulturalShare.Auth.API.csproj", "CulturalShare.Auth/"]
COPY ["./CulturalShare.Auth.Domain/CulturalShare.Auth.Domain.csproj", "CulturalShare.Auth.Domain/"]
COPY ["./CulturalShare.Auth.Repositories/CulturalShare.Auth.Repositories.csproj", "CulturalShare.Auth.Repositories/"]
COPY ["./CulturalShare.Auth.Services/CulturalShare.Auth.Services.csproj", "CulturalShare.Auth.Services/"]

# Restore dependencies
RUN dotnet restore "./CulturalShare.Auth/CulturalShare.Auth.API.csproj"

# Copy the rest of the project files
COPY . .

WORKDIR "/src/CulturalShare.Auth"

# Build the project
RUN dotnet build "./CulturalShare.Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

# Publish the project
RUN dotnet publish "./CulturalShare.Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Copy the published output to the final image
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "CulturalShare.Auth.API.dll"]
