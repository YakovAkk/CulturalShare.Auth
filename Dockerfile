# See https://aka.ms/customizecontainer to learn how to customize your debug container 
# and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG GITHUB_USERNAME
ENV GITHUB_USERNAME=$GITHUB_USERNAME
ARG GITHUB_TOKEN
ENV GITHUB_TOKEN=$GITHUB_TOKEN

WORKDIR /src

COPY Directory.Packages.props .

# Copy nuget.config and set GitHub credentials
COPY NuGet.Config .
RUN sed -i "s/%GITHUB_USERNAME%/${GITHUB_USERNAME}/g" NuGet.Config
RUN sed -i "s/%GITHUB_TOKEN%/${GITHUB_TOKEN}/g" NuGet.Config

# This stage is used to build the service project
COPY ["Web.Api/WebApi.csproj", "Web.Api/"]
COPY ["Dependency.Infranstructure/Dependency.Infranstructure.csproj", "Dependency.Infranstructure/"]
COPY ["Infrastructure/Postgres.Infrastructure.csproj", "Infrastructure/"]
COPY ["DomainEntity/DomainEntity.csproj", "DomainEntity/"]
COPY ["Repositories.Infrastructure/Repositories.Infrastructure.csproj", "Repositories.Infrastructure/"]
COPY ["Repository/Repository.csproj", "Repository/"]
COPY ["Service/Service.csproj", "Service/"]
RUN dotnet restore "./Web.Api/WebApi.csproj"
COPY . .
WORKDIR "/src/Web.Api"
RUN dotnet build "./WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]
