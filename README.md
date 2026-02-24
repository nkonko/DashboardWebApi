# WebAPI - Dashboard Web Application

API REST para gestión de Dashboard, Subscripciones, Configuraciones y Pagos.

## 🎯 Características

- **Dashboard**: Métricas y resúmenes (integración con IdentityAPI)
- **Subscriptions**: Gestión completa de suscripciones
- **Settings**: Configuraciones de la aplicación
- **Payments**: Procesamiento de webhooks de pagos
- **Health**: Endpoints de salud pública y autenticada

## 🛠️ Tecnologías

- .NET 10.0
- ASP.NET Core Web API
- Entity Framework Core 10.0
- PostgreSQL 16
- JWT Authentication
- Swagger UI + Scalar
- Docker & Docker Compose

## 🚀 Inicio Rápido

### Opción 1: Docker (Recomendado)

```bash
# Modo desarrollo (con hot-reload)
docker-compose -f docker-compose.dev.yml up --build

# Modo producción
docker-compose up --build
```

### Opción 2: Local

```bash
# Restaurar paquetes
dotnet restore

# Ejecutar migraciones (si tienes PostgreSQL local)
dotnet ef database update

# Ejecutar la aplicación
dotnet run
```

## 📍 Endpoints

### Acceso
- **API**: http://localhost:5002
- **Swagger**: http://localhost:5002/swagger
- **Scalar**: http://localhost:5002/scalar/v1

### PostgreSQL
- **Puerto**: 5433
- **Base de datos**: webapidb
- **Usuario/Contraseña**: postgres/postgres

### pgAdmin
- **Puerto**: 8086
- **Email**: admin@admin.com
- **Contraseña**: admin

## 🔐 Autenticación

Esta API **NO emite tokens JWT**, solo los valida. Para obtener un token:

1. Hacer login en **IdentityAPI** (puerto 5001)
2. Copiar el token recibido
3. Usar el token en WebAPI en el header: `Authorization: Bearer {token}`

## 📂 Estructura del Proyecto

```
WebApi/
├── Controllers/          # Controladores REST
├── Services/            # Lógica de negocio
│   └── Interfaces/      # Contratos de servicios
├── Entities/            # Entidades EF Core
├── Models/              # DTOs
├── Persistence/         # DbContext
├── Middleware/          # Middleware personalizado
├── Migrations/          # Migraciones EF Core
└── Program.cs           # Configuración de la app
```

## 🔄 Migraciones

Las migraciones se aplican automáticamente al iniciar la aplicación. Si necesitas crear una nueva:

```bash
dotnet ef migrations add MigrationName
```

## 🔗 Integración con IdentityAPI

WebAPI se comunica con IdentityAPI para obtener datos de usuarios y roles. Asegúrate de configurar:

```json
{
  "IdentityApiUrl": "http://localhost:5001"
}
```

## 📝 Variables de Entorno

```env
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Host=localhost;Port=5433;Database=webapidb;Username=postgres;Password=postgres
Jwt__Key=12345678901234567890123456789012
Jwt__Issuer=identityAPI
Jwt__Audience=identityAPIUsers
IdentityApiUrl=http://localhost:5001
```

## 🧪 Testing

```bash
# Health check público
curl http://localhost:5002/api/health

# Health check autenticado
curl -H "Authorization: Bearer {token}" http://localhost:5002/api/health/secure
```

## �️ Comandos Docker Útiles

### Ver logs
```bash
docker-compose logs -f webapi
```

### Detener servicios
```bash
docker-compose down
```

### Limpiar todo (incluyendo volúmenes)
```bash
docker-compose down -v
```

### Reconstruir contenedores
```bash
docker-compose up --build --force-recreate
```

## 📚 Documentación

- Swagger UI: http://localhost:5002/swagger
- Scalar UI: http://localhost:5002/scalar/v1
