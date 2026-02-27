# Etapa base: runtime de .NET
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_USE_POLLING_FILE_WATCHER=true \
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true \
    DOTNET_CLI_TELEMETRY_OPTOUT=true

# Etapa build: SDK para compilar y ejecutar EF Core
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiamos los .csproj primero para aprovechar cache en restore
COPY ["WebApi.Core/WebApi.Core.csproj", "WebApi.Core/"]
COPY ["WebApi.Infrastructure/WebApi.Infrastructure.csproj", "WebApi.Infrastructure/"]
COPY ["WebApi.Api/WebApi.Api.csproj", "WebApi.Api/"]

RUN dotnet restore "WebApi.Api/WebApi.Api.csproj"

# Copiamos todo el código y compilamos
COPY . .
WORKDIR "/src/WebApi.Api"
RUN dotnet build "WebApi.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa publish: genera artefactos optimizados
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebApi.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final para desarrollo (SDK incluido para EF Core)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS final
WORKDIR /app

# Copiamos la salida del publish
COPY --from=publish /app/publish .

# Crear usuario no root
RUN adduser --disabled-password --home /app appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "WebApi.dll"]
