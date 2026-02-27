# Etapa base: runtime de .NET
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Etapa build: SDK para compilar
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

# Etapa final: runtime optimizado
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.Api.dll"]
