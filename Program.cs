using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;
using WebApi.Middleware;
using WebApi.Persistence;
using Microsoft.EntityFrameworkCore;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

// Configuración de EF Core y PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<WebApiDbContext>(options =>
    options.UseNpgsql(connectionString));

// Registro automático de servicios de aplicación
builder.Services.AddApplicationServices();

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

// Configure JWT Authentication (validation only)
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

// 👉 CORS SOLO para desarrollo
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularDev",
            policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
    });
}

// Swagger con soporte JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WebAPI",
        Version = "v1",
        Description = "API for Dashboard, Subscriptions, Settings and Payments"
    });

    // Configure JWT authentication in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Add HttpClient for calling IdentityAPI
builder.Services.AddHttpClient("IdentityAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["IdentityApiUrl"]!);
});

var app = builder.Build();

// ✨ Middleware de manejo global de excepciones
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Espera a que la base de datos esté lista y aplica migraciones
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WebApiDbContext>();
    int retries = 0;
    int maxRetries = 10;
    int delaySeconds = 5;
    while (true)
    {
        try
        {
            await context.Database.OpenConnectionAsync();
            await context.Database.CloseConnectionAsync();
            break;
        }
        catch (Npgsql.NpgsqlException)
        {
            retries++;
            if (retries >= maxRetries)
                throw;
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        }
    }
    await context.Database.MigrateAsync();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI v1");
    });
    
    // Map Scalar UI
    app.MapScalarApiReference();

    // 👉 Activamos CORS solo en desarrollo
    app.UseCors("AllowAngularDev");
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 🚀 Mensaje de inicio para desarrollo
if (app.Environment.IsDevelopment())
{
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        Console.WriteLine("");
        Console.WriteLine("🚀 WebAPI está corriendo!");
        Console.WriteLine("📍 Swagger UI: http://localhost:5002/swagger");
        Console.WriteLine("📍 Scalar UI: http://localhost:5002/scalar/v1");
        Console.WriteLine("📍 API Base: http://localhost:5002");
        Console.WriteLine("🔧 Modo desarrollo con hot-reload activado");
        Console.WriteLine("🔗 IdentityAPI: " + builder.Configuration["IdentityApiUrl"]);
        Console.WriteLine("💡 Haz Ctrl+Click en las URLs para abrirlas en tu navegador");
        Console.WriteLine("");
    });
}

app.Run();


