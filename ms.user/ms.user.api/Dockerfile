# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ms.user/ms.user.api/ms.user.api.csproj", "ms.user/ms.user.api/"]
COPY ["ms.communication/ms.rabbitmq/ms.rabbitmq.csproj", "ms.communication/ms.rabbitmq/"]
COPY ["ms.user/ms.user.application/ms.user.application.csproj", "ms.user/ms.user.application/"]
COPY ["ms.user/ms.user.domain/ms.user.domain.csproj", "ms.user/ms.user.domain/"]
COPY ["ms.user/ms.user.infrastructure/ms.user.infrastructure.csproj", "ms.user/ms.user.infrastructure/"]
RUN dotnet restore "./ms.user/ms.user.api/ms.user.api.csproj"
COPY . .
WORKDIR "/src/ms.user/ms.user.api"
RUN dotnet build "./ms.user.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ms.user.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ms.user.api.dll"]